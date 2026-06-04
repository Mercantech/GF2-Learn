/**
 * Lightweight C# symbol extraction for Monaco completion (variables, methods, classes).
 */
(function (global) {
  var RESERVED = {
    abstract: 1, as: 1, base: 1, bool: 1, break: 1, byte: 1, case: 1, catch: 1, char: 1,
    checked: 1, class: 1, const: 1, continue: 1, decimal: 1, default: 1, delegate: 1, do: 1,
    double: 1, else: 1, enum: 1, event: 1, explicit: 1, extern: 1, false: 1, finally: 1,
    fixed: 1, float: 1, for: 1, foreach: 1, goto: 1, if: 1, implicit: 1, in: 1, int: 1,
    interface: 1, internal: 1, is: 1, lock: 1, long: 1, namespace: 1, new: 1, null: 1,
    object: 1, operator: 1, out: 1, override: 1, params: 1, private: 1, protected: 1,
    public: 1, readonly: 1, ref: 1, return: 1, sbyte: 1, sealed: 1, short: 1, sizeof: 1,
    stackalloc: 1, static: 1, string: 1, struct: 1, switch: 1, this: 1, throw: 1, true: 1,
    try: 1, typeof: 1, uint: 1, ulong: 1, unchecked: 1, unsafe: 1, ushort: 1, using: 1,
    virtual: 1, void: 1, volatile: 1, while: 1, var: 1, add: 1, alias: 1, ascending: 1,
    async: 1, await: 1, by: 1, descending: 1, dynamic: 1, equals: 1, from: 1, get: 1,
    global: 1, group: 1, into: 1, join: 1, let: 1, nameof: 1, on: 1, orderby: 1,
    partial: 1, remove: 1, select: 1, set: 1, value: 1, when: 1, where: 1, yield: 1,
    Console: 1, Math: 1, Convert: 1, List: 1, Dictionary: 1, Array: 1, String: 1,
    DateTime: 1, Random: 1, Exception: 1
  };

  var MEMBER_MAP = {
    string: ["Length", "ToLower", "ToUpper", "Trim", "Substring", "Contains", "StartsWith", "EndsWith", "Replace", "Split"],
    int: ["ToString"],
    double: ["ToString"],
    bool: ["ToString"],
    List: ["Add", "Count", "Remove", "Clear", "Contains", "IndexOf"],
    Dictionary: ["Add", "Count", "Remove", "ContainsKey", "TryGetValue"],
    Console: ["WriteLine", "Write", "ReadLine"],
    Math: ["PI", "Abs", "Max", "Min", "Round", "Floor", "Ceiling", "Pow", "Sqrt"],
    Random: ["Next", "NextDouble"]
  };

  function resolveSymbolType(symbols, objectName) {
    for (var i = symbols.length - 1; i >= 0; i--) {
      if (symbols[i].name !== objectName) continue;
      var detail = symbols[i].detail || "";
      var typePart = detail.split(" · ")[0].trim();
      if (typePart === "foreach" || typePart === "for" || typePart === "primitive") {
        return "var";
      }
      if (typePart.indexOf("List<") === 0 || typePart === "List") return "List";
      if (typePart.indexOf("Dictionary<") === 0 || typePart === "Dictionary") return "Dictionary";
      if (typePart === "string" || typePart === "int" || typePart === "double" || typePart === "bool") {
        return typePart;
      }
      return typePart.split("<")[0].split("[")[0];
    }
    if (MEMBER_MAP[objectName]) return objectName;
    return null;
  }

  function memberSuggestions(monaco, objectName, allDocSymbols, range, prefix) {
    var typeKey = resolveSymbolType(allDocSymbols, objectName);
    var members = MEMBER_MAP[typeKey] || MEMBER_MAP[objectName] || [];
    var out = [];

    for (var m = 0; m < members.length; m++) {
      var mem = members[m];
      if (prefix && mem.toLowerCase().indexOf(prefix) !== 0) continue;
      out.push({
        label: mem,
        kind: monaco.languages.CompletionItemKind.Method,
        detail: (typeKey || objectName) + "." + mem,
        insertText: mem,
        range: range,
        sortText: "0_" + mem
      });
    }
    return out;
  }

  var BUILTINS = [
    { name: "Console", kind: "class", detail: "System.Console" },
    { name: "Math", kind: "class", detail: "System.Math" },
    { name: "Convert", kind: "class", detail: "System.Convert" },
    { name: "List", kind: "class", detail: "System.Collections.Generic.List<T>" },
    { name: "Dictionary", kind: "class", detail: "System.Collections.Generic.Dictionary<K,V>" },
    { name: "DateTime", kind: "class", detail: "System.DateTime" },
    { name: "Random", kind: "class", detail: "System.Random" },
    { name: "string", kind: "type", detail: "System.String" },
    { name: "int", kind: "type", detail: "System.Int32" },
    { name: "double", kind: "type", detail: "System.Double" },
    { name: "bool", kind: "type", detail: "System.Boolean" },
    { name: "decimal", kind: "type", detail: "System.Decimal" },
    { name: "char", kind: "type", detail: "System.Char" },
    { name: "var", kind: "keyword", detail: "implicit type" }
  ];

  function isIdent(name) {
    return name && /^[A-Za-z_]\w*$/.test(name) && !RESERVED[name];
  }

  function braceDepthBefore(lines, lineIndex, column) {
    var depth = 0;
    var li;
    for (li = 0; li < lineIndex; li++) {
      var line = lines[li];
      for (var c = 0; c < line.length; c++) {
        if (line[c] === "{") depth++;
        else if (line[c] === "}") depth--;
      }
    }
    if (lineIndex < lines.length) {
      var cur = lines[lineIndex];
      var col = column == null ? cur.length : column;
      for (var j = 0; j < col; j++) {
        if (cur[j] === "{") depth++;
        else if (cur[j] === "}") depth--;
      }
    }
    return depth;
  }

  function addSymbol(symbols, seen, entry) {
    if (!isIdent(entry.name)) return;
    var key = entry.kind + ":" + entry.name;
    if (seen[key]) return;
    seen[key] = true;
    symbols.push(entry);
  }

  function scanLine(lines, lineIndex, symbols, seen) {
    var line = lines[lineIndex];
    var trimmed = line.trim();
    if (!trimmed || trimmed.indexOf("//") === 0) return;

    var patterns = [
      {
        re: /^\s*(?:public|private|protected|internal)?\s*(?:static\s+)?class\s+(\w+)/,
        kind: "class",
        type: "class",
        g: 1
      },
      {
        re: /^\s*(?:public|private|protected|internal)?\s*(?:static\s+)?void\s+(\w+)\s*\(/,
        kind: "method",
        type: "void",
        g: 1
      },
      {
        re: /^\s*(?:public|private|protected|internal)?\s*(?:static\s+)?(?:async\s+)?([\w<>,\[\]]+)\s+(\w+)\s*\(/,
        kind: "method",
        typeFrom: 1,
        nameFrom: 2
      },
      {
        re: /^\s*(?:public|private|protected|internal)?\s*([\w<>,\[\]]+)\s+(\w+)\s*\{\s*get/,
        kind: "property",
        typeFrom: 1,
        nameFrom: 2
      },
      {
        re: /foreach\s*\(\s*(?:var|[\w<>,\[\]]+)\s+(\w+)\s+in/,
        kind: "variable",
        type: "foreach",
        g: 1
      },
      {
        re: /for\s*\(\s*(?:var|int|[\w<>,\[\]]+)\s+(\w+)\s*=/,
        kind: "variable",
        type: "for",
        g: 1
      },
      {
        re: /^\s*(?:var|int|string|bool|double|float|decimal|char|long|byte|short)\s+(\w+)\s*=/,
        kind: "variable",
        type: "primitive",
        g: 1
      },
      {
        re: /^\s*(List|Dictionary)(<[^>]+>)\s+(\w+)\s*=/,
        kind: "variable",
        typeFrom: 1,
        typeSuffixFrom: 2,
        nameFrom: 3
      },
      {
        re: /^\s*([\w<>,\[\]]+)\s*\[\s*\]\s+(\w+)\s*=/,
        kind: "variable",
        typeFrom: 1,
        typeSuffix: "[]",
        nameFrom: 2
      },
      {
        re: /^\s*([\w<>,\[\]]+)\s+(\w+)\s*=\s*(?!=)/,
        kind: "variable",
        typeFrom: 1,
        nameFrom: 2
      }
    ];

    var colBase = line.indexOf(trimmed);
    var depth = braceDepthBefore(lines, lineIndex, colBase);

    for (var p = 0; p < patterns.length; p++) {
      var pat = patterns[p];
      var m = pat.re.exec(line);
      if (!m) continue;

      var name;
      var typeLabel;
      if (pat.g != null) {
        name = m[pat.g];
        typeLabel = pat.type || "";
      } else {
        name = m[pat.nameFrom];
        typeLabel = m[pat.typeFrom] || "";
        if (pat.typeSuffixFrom) typeLabel += m[pat.typeSuffixFrom];
        if (pat.typeSuffix) typeLabel += pat.typeSuffix;
      }

      if (!isIdent(name)) continue;
      if (typeLabel === "get" || typeLabel === "set") continue;

      var col = line.indexOf(name, colBase);
      addSymbol(symbols, seen, {
        name: name,
        kind: pat.kind,
        detail: typeLabel ? typeLabel + " · linje " + (lineIndex + 1) : "linje " + (lineIndex + 1),
        line: lineIndex + 1,
        column: col >= 0 ? col + 1 : 1,
        depth: depth
      });
      break;
    }
  }

  function collectDocumentSymbols(text) {
    var lines = text.replace(/\r\n/g, "\n").split("\n");
    var symbols = [];
    var seen = {};

    for (var i = 0; i < lines.length; i++) {
      scanLine(lines, i, symbols, seen);
    }

    return symbols;
  }

  function symbolsInScope(symbols, lines, position) {
    var cursorLine = position.lineNumber;
    var cursorCol = position.column;
    var cursorDepth = braceDepthBefore(lines, cursorLine - 1, cursorCol - 1);

    return symbols.filter(function (sym) {
      if (sym.line > cursorLine) return false;
      if (sym.line === cursorLine && sym.column >= cursorCol) return false;
      return sym.depth <= cursorDepth;
    });
  }

  function kindToMonaco(monaco, kind) {
    switch (kind) {
      case "method":
        return monaco.languages.CompletionItemKind.Method;
      case "class":
        return monaco.languages.CompletionItemKind.Class;
      case "property":
        return monaco.languages.CompletionItemKind.Property;
      case "type":
        return monaco.languages.CompletionItemKind.Class;
      case "keyword":
        return monaco.languages.CompletionItemKind.Keyword;
      default:
        return monaco.languages.CompletionItemKind.Variable;
    }
  }

  function buildSuggestions(monaco, model, position, snippets) {
    var word = model.getWordUntilPosition(position);
    var range = {
      startLineNumber: position.lineNumber,
      endLineNumber: position.lineNumber,
      startColumn: word.startColumn,
      endColumn: word.endColumn
    };

    var prefix = (word.word || "").toLowerCase();
    var sourceText = model.getValue();
    var lines = sourceText.replace(/\r\n/g, "\n").split("\n");
    var allDocSymbols = collectDocumentSymbols(sourceText);
    var docSymbols = symbolsInScope(allDocSymbols, lines, position);

    var lineContent = model.getLineContent(position.lineNumber);
    var beforeCursor = lineContent.substring(0, position.column - 1);
    var dotMatch = beforeCursor.match(/(\w+)\.\s*$/);
    if (dotMatch) {
      var memberItems = memberSuggestions(
        monaco,
        dotMatch[1],
        allDocSymbols,
        range,
        prefix
      );
      if (memberItems.length) {
        return { suggestions: memberItems };
      }
    }

    var suggestions = [];
    var used = {};

    function matches(name) {
      if (!prefix) return true;
      return name.toLowerCase().indexOf(prefix) === 0;
    }

    function push(item) {
      var key = item.label + ":" + item.kind;
      if (used[key]) return;
      used[key] = true;
      suggestions.push(item);
    }

    for (var i = 0; i < docSymbols.length; i++) {
      var sym = docSymbols[i];
      if (!matches(sym.name)) continue;
      push({
        label: sym.name,
        kind: kindToMonaco(monaco, sym.kind),
        detail: sym.detail,
        insertText: sym.name,
        range: range,
        sortText: "1_" + sym.name
      });
    }

    for (var b = 0; b < BUILTINS.length; b++) {
      var bi = BUILTINS[b];
      if (!matches(bi.name)) continue;
      push({
        label: bi.name,
        kind: kindToMonaco(monaco, bi.kind),
        detail: bi.detail,
        insertText: bi.name,
        range: range,
        sortText: "2_" + bi.name
      });
    }

    for (var s = 0; s < snippets.length; s++) {
      var sn = snippets[s];
      if (!matches(sn.label)) continue;
      push({
        label: sn.label,
        kind: monaco.languages.CompletionItemKind.Snippet,
        detail: sn.detail,
        insertText: sn.insertText,
        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
        range: range,
        sortText: "0_" + sn.label
      });
    }

    return { suggestions: suggestions };
  }

  global.gf2CSharpIntellisense = {
    collectDocumentSymbols: collectDocumentSymbols,
    buildSuggestions: buildSuggestions
  };
})(window);
