window.gf2Playground = {
  editors: {},
  editorOptions: {},
  monacoLoadPromise: null,
  monacoBase: "https://cdn.jsdelivr.net/npm/monaco-editor@0.52.2/min/vs",

  csharpSnippets: [
    { label: "cw", detail: "Console.WriteLine(...)", insertText: "Console.WriteLine(${1});" },
    { label: "cwl", detail: "Console.WriteLine med tekst", insertText: 'Console.WriteLine("${1}");' },
    { label: "for", detail: "for-loop", insertText: "for (int ${1:i} = 0; ${1:i} < ${2:length}; ${1:i}++)\n{\n\t${0}\n}" },
    { label: "foreach", detail: "foreach-loop", insertText: "foreach (var ${1:item} in ${2:collection})\n{\n\t${0}\n}" },
    { label: "if", detail: "if-sætning", insertText: "if (${1:condition})\n{\n\t${0}\n}" },
    { label: "else", detail: "else", insertText: "else\n{\n\t${0}\n}" },
    { label: "class", detail: "klasse", insertText: "class ${1:Name}\n{\n\t${0}\n}" },
    { label: "main", detail: "Main metode", insertText: "static void Main(string[] args)\n{\n\t${0}\n}" }
  ],

  loadMonaco: function () {
    if (window.monaco) return Promise.resolve(window.monaco);
    if (this.monacoLoadPromise) return this.monacoLoadPromise;

    this.monacoLoadPromise = new Promise(function (resolve, reject) {
      var base = window.gf2Playground.monacoBase;
      var onReady = function () {
        window.require.config({ paths: { vs: base } });
        window.require(["vs/editor/editor.main"], function () {
          window.gf2Playground.defineTheme();
          window.gf2Playground.registerCSharpSupport();
          resolve(window.monaco);
        }, reject);
      };

      if (window.require) {
        onReady();
        return;
      }

      var loader = document.createElement("script");
      loader.src = base + "/loader.js";
      loader.async = true;
      loader.onload = onReady;
      loader.onerror = reject;
      document.head.appendChild(loader);
    });

    return this.monacoLoadPromise;
  },

  resolveTheme: function () {
    if (window.gf2Theme && window.gf2Theme.getEffective) {
      return window.gf2Theme.getEffective() === "dark" ? "gf2-dark" : "gf2-light";
    }
    var mode = document.documentElement.getAttribute("data-theme");
    return mode === "dark" ? "gf2-dark" : "gf2-light";
  },

  defineTheme: function () {
    if (window.gf2Playground._themeDefined) return;
    var monaco = window.monaco;
    monaco.editor.defineTheme("gf2-light", {
      base: "vs",
      inherit: true,
      rules: [],
      colors: {
        "editor.background": "#f1f5f9",
        "editor.lineHighlightBackground": "#e2e8f0",
        "editorLineNumber.foreground": "#94a3b8",
        "editorCursor.foreground": "#0284c7",
        "editor.selectionBackground": "#bae6fd"
      }
    });
    monaco.editor.defineTheme("gf2-dark", {
      base: "vs-dark",
      inherit: true,
      rules: [],
      colors: {
        "editor.background": "#0f1419",
        "editor.lineHighlightBackground": "#1a2332",
        "editorLineNumber.foreground": "#4b5563",
        "editorCursor.foreground": "#22d3ee",
        "editor.selectionBackground": "#264f78"
      }
    });
    window.gf2Playground._themeDefined = true;
  },

  applyTheme: function () {
    if (!window.monaco || !window.gf2Playground._themeDefined) return;
    window.monaco.editor.setTheme(this.resolveTheme());
  },

  registerCSharpSupport: function () {
    if (window.gf2Playground._csharpRegistered) return;
    window.gf2Playground._csharpRegistered = true;

    var monaco = window.monaco;
    var snippets = window.gf2Playground.csharpSnippets;

    monaco.languages.registerCompletionItemProvider("csharp", {
      triggerCharacters: ["."],
      provideCompletionItems: function (model, position) {
        var word = model.getWordUntilPosition(position);
        var range = {
          startLineNumber: position.lineNumber,
          endLineNumber: position.lineNumber,
          startColumn: word.startColumn,
          endColumn: word.endColumn
        };

        return {
          suggestions: snippets.map(function (s) {
            return {
              label: s.label,
              kind: monaco.languages.CompletionItemKind.Snippet,
              detail: s.detail,
              insertText: s.insertText,
              insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
              range: range,
              sortText: "0_" + s.label
            };
          })
        };
      }
    });
  },

  registerTabSnippets: function (editor) {
    var monaco = window.monaco;
    var snippets = this.csharpSnippets;

    editor.addCommand(monaco.KeyCode.Tab, function () {
      var model = editor.getModel();
      var position = editor.getPosition();
      if (!model || !position) {
        editor.trigger("keyboard", "type", { text: "\t" });
        return;
      }

      var line = model.getLineContent(position.lineNumber);
      var before = line.substring(0, position.column - 1);

      for (var i = 0; i < snippets.length; i++) {
        var s = snippets[i];
        var re = new RegExp("\\b" + s.label.replace(/[.*+?^${}()|[\]\\]/g, "\\$&") + "$");
        if (re.test(before)) {
          var startCol = position.column - s.label.length;
          editor.executeEdits("snippet", [{
            range: new monaco.Range(position.lineNumber, startCol, position.lineNumber, position.column),
            text: "",
            forceMoveMarkers: true
          }]);
          editor.trigger("keyboard", "editor.action.insertSnippet", {
            snippet: s.insertText
          });
          return;
        }
      }

      editor.trigger("keyboard", "type", { text: "\t" });
    });
  },

  resolveHeightOptions: function (host, options) {
    options = options || {};
    var isExercise = host.classList.contains("playground-editor-host-exercise");
    var isCompact = host.classList.contains("playground-editor-host-compact");

    return {
      minLines: options.minLines != null ? options.minLines : (isExercise ? 5 : isCompact ? 6 : 8),
      maxLines: options.maxLines != null ? options.maxLines : (isExercise ? 36 : isCompact ? 28 : 48),
      extraLines: options.extraLines != null ? options.extraLines : 1.5,
      paddingTop: options.paddingTop != null ? options.paddingTop : 12,
      paddingBottom: options.paddingBottom != null ? options.paddingBottom : 12
    };
  },

  updateHeight: function (elementId) {
    var editor = this.editors[elementId];
    var host = document.getElementById(elementId);
    var opts = this.editorOptions[elementId];
    if (!editor || !host || !opts) return;

    var width = host.clientWidth;
    if (width <= 0) {
      this.scheduleRelayout(elementId);
      return;
    }

    var monaco = window.monaco;
    var lineHeight = editor.getOption(monaco.editor.EditorOption.lineHeight);
    var pad = opts.paddingTop + opts.paddingBottom;
    var extraPx = opts.extraLines * lineHeight;
    var minPx = opts.minLines * lineHeight + pad;
    var maxPx = opts.maxLines * lineHeight + pad;

    var contentHeight = editor.getContentHeight();
    var target = Math.min(maxPx, Math.max(minPx, contentHeight + extraPx));

    host.style.height = target + "px";
    editor.layout({ width: width, height: target });
  },

  scheduleRelayout: function (elementId) {
    var self = this;
    var delays = [0, 0, 50, 150, 400];
    delays.forEach(function (ms, idx) {
      if (idx === 0) {
        requestAnimationFrame(function () {
          self.updateHeight(elementId);
        });
        return;
      }
      if (idx === 1) {
        requestAnimationFrame(function () {
          requestAnimationFrame(function () {
            self.updateHeight(elementId);
          });
        });
        return;
      }
      setTimeout(function () {
        self.updateHeight(elementId);
      }, ms);
    });
  },

  isReady: function (elementId) {
    return !!this.editors[elementId];
  },

  layoutVisible: function () {
    var ids = Object.keys(this.editors);
    for (var i = 0; i < ids.length; i++) {
      this.scheduleRelayout(ids[i]);
    }
  },

  layoutVisibleDelayed: function () {
    var self = this;
    self.layoutVisible();
    setTimeout(function () {
      self.layoutVisible();
    }, 250);
    setTimeout(function () {
      self.layoutVisible();
    }, 600);
  },

  init: async function (elementId, initialCode, options) {
    options = options || {};
    await this.loadMonaco();

    var host = document.getElementById(elementId);
    if (!host) return;

    if (this.editors[elementId]) {
      this.dispose(elementId);
    }

    host.textContent = "";

    var heightOpts = this.resolveHeightOptions(host, options);
    this.editorOptions[elementId] = heightOpts;

    var monaco = window.monaco;
    var editor = monaco.editor.create(host, {
      value: initialCode || "",
      language: "csharp",
      theme: this.resolveTheme(),
      automaticLayout: false,
      fontFamily: "JetBrains Mono, Consolas, monospace",
      fontSize: 14,
      lineHeight: 22,
      minimap: { enabled: false },
      scrollBeyondLastLine: false,
      wordWrap: "on",
      tabSize: 4,
      insertSpaces: true,
      readOnly: !!options.readOnly,
      renderLineHighlight: "line",
      padding: { top: heightOpts.paddingTop, bottom: heightOpts.paddingBottom },
      quickSuggestions: { other: true, comments: false, strings: false },
      suggestOnTriggerCharacters: true,
      snippetSuggestions: "top",
      scrollbar: {
        vertical: "auto",
        horizontal: "hidden",
        useShadows: false
      }
    });

    var self = this;
    editor.onDidContentSizeChange(function () {
      self.updateHeight(elementId);
    });

    window.addEventListener("resize", function onResize() {
      if (!self.editors[elementId]) {
        window.removeEventListener("resize", onResize);
        return;
      }
      self.updateHeight(elementId);
    });

    this.registerTabSnippets(editor);
    this.editors[elementId] = editor;
    this.scheduleRelayout(elementId);
  },

  getValue: function (elementId) {
    var editor = this.editors[elementId];
    return editor ? editor.getValue() : "";
  },

  setValue: function (elementId, code) {
    var editor = this.editors[elementId];
    if (!editor) return;
    editor.setValue(code || "");
    this.updateHeight(elementId);
  },

  setReadOnly: function (elementId, readOnly) {
    var editor = this.editors[elementId];
    if (editor) editor.updateOptions({ readOnly: readOnly });
  },

  dispose: function (elementId) {
    var editor = this.editors[elementId];
    if (!editor) return;
    editor.dispose();
    delete this.editors[elementId];
    delete this.editorOptions[elementId];
    var host = document.getElementById(elementId);
    if (host) host.textContent = "";
  },

  disposeAll: function () {
    var ids = Object.keys(this.editors);
    for (var i = 0; i < ids.length; i++) {
      this.dispose(ids[i]);
    }
  }
};
