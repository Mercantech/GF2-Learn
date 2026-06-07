window.gf2Playground = {
  editors: {},
  editorOptions: {},
  ctrlRunHelpers: {},
  ctrlSaveHelpers: {},
  keyboardBound: {},
  lazyObservers: {},
  heightSyncScheduled: {},
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
    this.refreshAll();
  },

  registerCSharpSupport: function () {
    if (window.gf2Playground._csharpRegistered) return;
    window.gf2Playground._csharpRegistered = true;

    var monaco = window.monaco;
    var snippets = window.gf2Playground.csharpSnippets;

    monaco.languages.registerCompletionItemProvider("csharp", {
      triggerCharacters: ["."],
      provideCompletionItems: function (model, position) {
        try {
          if (window.gf2CSharpIntellisense && window.gf2CSharpIntellisense.buildSuggestions) {
            return window.gf2CSharpIntellisense.buildSuggestions(
              monaco,
              model,
              position,
              snippets
            );
          }
        } catch (e) {
          console.warn("gf2 C# intellisense:", e);
        }

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

  registerFormatCommand: function (elementId, host) {
    var editor = this.editors[elementId];
    if (!editor || !window.monaco) return;

    var monaco = window.monaco;
    var self = this;

    var runFormat = function () {
      self.formatDocument(elementId);
    };

    editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyMod.Shift | monaco.KeyCode.KeyF, runFormat);
    editor.addCommand(monaco.KeyMod.Shift | monaco.KeyMod.Alt | monaco.KeyCode.KeyF, runFormat);

    if (!host || host.dataset.formatBound === "1") return;
    host.dataset.formatBound = "1";

    host.addEventListener("keydown", function (e) {
      var ctrlFmt = (e.ctrlKey || e.metaKey) && e.shiftKey && (e.key === "F" || e.key === "f");
      var altShiftFmt = e.shiftKey && e.altKey && (e.key === "F" || e.key === "f");
      if (!ctrlFmt && !altShiftFmt) return;
      if (!editor.hasTextFocus || !editor.hasTextFocus()) return;
      e.preventDefault();
      e.stopPropagation();
      runFormat();
    }, true);
  },

  formatDocument: async function (elementId) {
    var editor = this.editors[elementId];
    if (!editor) return false;

    var code = editor.getValue();
    if (!code.trim()) return false;

    var model = editor.getModel();
    if (!model) return false;

    try {
      var response = await fetch("/api/playground/format", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ code: code })
      });

      if (!response.ok) {
        var detail = "Formatering fejlede.";
        try {
          var err = await response.json();
          if (err && err.detail) detail = err.detail;
        } catch (ignore) { /* ignore */ }
        console.warn("gf2 format:", detail);
        return false;
      }

      var data = await response.json();
      if (!data || typeof data.formatted !== "string") return false;
      if (data.formatted === code) return true;

      var fullRange = model.getFullModelRange();
      editor.pushUndoStop();
      editor.executeEdits("format", [{
        range: fullRange,
        text: data.formatted,
        forceMoveMarkers: true
      }]);
      editor.pushUndoStop();
      this.updateHeight(elementId);
      return true;
    } catch (e) {
      console.warn("gf2 format:", e);
      return false;
    }
  },

  getPlaygroundRoot: function (elementId) {
    var host = document.getElementById(elementId);
    return host ? host.closest(".code-playground") : null;
  },

  isEditorHostActive: function (elementId) {
    var host = document.getElementById(elementId);
    return !!(host && host.contains(document.activeElement));
  },

  clickPlaygroundButton: function (elementId, selectors) {
    var root = this.getPlaygroundRoot(elementId);
    if (!root) return false;

    var list = Array.isArray(selectors) ? selectors : [selectors];
    for (var i = 0; i < list.length; i++) {
      var btn = root.querySelector(list[i]);
      if (btn && !btn.disabled) {
        btn.click();
        return true;
      }
    }
    return false;
  },

  invokeCtrlSave: function (elementId) {
    var helper = this.ctrlSaveHelpers[elementId];
    if (!helper) return;

    helper.invokeMethodAsync("TriggerCtrlSaveAsync").catch(function (err) {
      console.warn("gf2 Ctrl+S save:", err);
    });
  },

  invokeCtrlRun: function (elementId) {
    var helper = this.ctrlRunHelpers[elementId];
    if (!helper) return;

    helper.invokeMethodAsync("TriggerCtrlRunAsync").catch(function (err) {
      console.warn("gf2 Ctrl+Enter run:", err);
    });
  },

  triggerRun: function (elementId) {
    if (
      this.clickPlaygroundButton(elementId, [
        ".playground-icon-btn-run",
        ".playground-header-toolbar .btn-primary"
      ])
    ) {
      return;
    }
    this.invokeCtrlRun(elementId);
  },

  triggerSave: function (elementId) {
    if (this.clickPlaygroundButton(elementId, ".playground-icon-btn-save")) return;
    this.invokeCtrlSave(elementId);
  },

  bindKeyboardShortcuts: function (elementId) {
    var editor = this.editors[elementId];
    var host = document.getElementById(elementId);
    if (!editor || !host || this.keyboardBound[elementId]) return;

    var self = this;
    var monaco = window.monaco;

    var runCode = function () {
      self.triggerRun(elementId);
    };

    var saveCode = function () {
      self.triggerSave(elementId);
    };

    if (monaco) {
      editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.Enter, runCode);
      editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.NumpadEnter, runCode);
      editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyS, saveCode);
    }

    host.addEventListener(
      "keydown",
      function (e) {
        if (!self.isEditorHostActive(elementId)) return;

        var ctrl = e.ctrlKey || e.metaKey;
        if (!ctrl) return;

        if (e.key === "Enter") {
          e.preventDefault();
          e.stopPropagation();
          runCode();
          return;
        }

        if (e.key === "s" || e.key === "S") {
          e.preventDefault();
          e.stopPropagation();
          saveCode();
        }
      },
      true
    );

    this.keyboardBound[elementId] = true;
  },

  registerCtrlSave: function (elementId, dotNetHelper) {
    if (dotNetHelper) this.ctrlSaveHelpers[elementId] = dotNetHelper;
    this.bindKeyboardShortcuts(elementId);
  },

  registerCtrlRun: function (elementId, dotNetHelper) {
    if (dotNetHelper) this.ctrlRunHelpers[elementId] = dotNetHelper;
    this.bindKeyboardShortcuts(elementId);
  },

  registerTabSnippets: function (editor) {
    var monaco = window.monaco;
    var snippets = this.csharpSnippets;

    editor.addCommand(monaco.KeyCode.Tab, function () {
      var model = editor.getModel();
      var position = editor.getPosition();

      if (model && position) {
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
      }

      // Normal indrykning (4 mellemrum) — ikke et råt tab-tegn
      editor.trigger("keyboard", "tab", {});
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

  hostIsReady: function (host) {
    if (!host || !host.isConnected) return false;
    var rect = host.getBoundingClientRect();
    return rect.width >= 48;
  },

  waitForHostReady: function (host, maxMs) {
    var self = this;
    maxMs = maxMs || 2000;
    return new Promise(function (resolve) {
      if (self.hostIsReady(host)) {
        resolve();
        return;
      }

      var attempts = 0;
      var maxAttempts = Math.ceil(maxMs / 50);
      var timer = setInterval(function () {
        attempts++;
        if (self.hostIsReady(host) || attempts >= maxAttempts) {
          clearInterval(timer);
          resolve();
        }
      }, 50);
    });
  },

  applyContentHeight: function (elementId, contentHeight) {
    var editor = this.editors[elementId];
    var host = document.getElementById(elementId);
    var opts = this.editorOptions[elementId];
    if (!editor || !host || !opts) return;

    var monaco = window.monaco;
    var lineHeight = editor.getOption(monaco.editor.EditorOption.lineHeight);
    var pad = opts.paddingTop + opts.paddingBottom;
    var extraPx = opts.extraLines * lineHeight;
    var minPx = opts.minLines * lineHeight + pad;
    var maxPx = opts.maxLines * lineHeight + pad;
    var measured = contentHeight > 0 ? contentHeight : editor.getContentHeight();
    var target = Math.min(maxPx, Math.max(minPx, measured + extraPx));
    var prev = parseInt(host.style.height, 10);

    if (!isNaN(prev) && Math.abs(prev - target) < 2) {
      host.dataset.editorReady = "1";
      return;
    }

    host.style.height = target + "px";
    host.dataset.editorReady = "1";
  },

  markHostReady: function (host) {
    if (!host) return;
    host.dataset.editorReady = "1";
    host.classList.remove("playground-editor-host-loading");
  },

  scheduleHeightSync: function (elementId) {
    if (this.heightSyncScheduled[elementId]) return;
    this.heightSyncScheduled[elementId] = true;
    var self = this;
    requestAnimationFrame(function () {
      delete self.heightSyncScheduled[elementId];
      var editor = self.editors[elementId];
      if (!editor) return;
      self.applyContentHeight(elementId, editor.getContentHeight());
    });
  },

  updateHeight: function (elementId) {
    this.scheduleHeightSync(elementId);
  },

  detachLazyObserver: function (elementId) {
    var io = this.lazyObservers[elementId];
    if (!io) return;
    io.disconnect();
    delete this.lazyObservers[elementId];
  },

  isReady: function (elementId) {
    var host = document.getElementById(elementId);
    var editor = this.editors[elementId];
    if (!editor || !host) return false;
    if (this.hostHasEditor(host)) {
      host.dataset.editorReady = "1";
      host.classList.remove("playground-editor-host-loading");
      return true;
    }
    return host.dataset.editorReady === "1";
  },

  refreshAll: function () {
    var ids = Object.keys(this.editors);
    for (var i = 0; i < ids.length; i++) {
      this.updateHeight(ids[i]);
    }
  },

  layoutVisibleDelayed: function () {
    this.refreshAll();
  },

  init: async function (elementId, initialCode, options) {
    options = options || {};
    await this.loadMonaco();

    var host = document.getElementById(elementId);
    if (!host) return;

    this.dispose(elementId);
    host.textContent = "";
    host.dataset.editorReady = "0";
    host.classList.add("playground-editor-host-loading");

    await this.waitForHostReady(host);

    var heightOpts = this.resolveHeightOptions(host, options);
    this.editorOptions[elementId] = heightOpts;

    var monaco = window.monaco;
    var self = this;
    var editor = monaco.editor.create(host, {
      value: initialCode || "",
      language: "csharp",
      theme: this.resolveTheme(),
      automaticLayout: true,
      fixedOverflowWidgets: true,
      fontFamily: "JetBrains Mono, Consolas, monospace",
      fontSize: 14,
      lineHeight: 22,
      minimap: { enabled: false },
      scrollBeyondLastLine: false,
      wordWrap: "on",
      tabSize: 4,
      insertSpaces: true,
      readOnly: !!options.readOnly,
      lineNumbers: "on",
      renderLineHighlight: "line",
      padding: { top: heightOpts.paddingTop, bottom: heightOpts.paddingBottom },
      quickSuggestions: { other: true, comments: false, strings: false },
      suggestOnTriggerCharacters: true,
      wordBasedSuggestions: "off",
      suggest: {
        showWords: false,
        preview: true,
        filterGraceful: true,
        localityBonus: true
      },
      snippetSuggestions: "top",
      parameterHints: { enabled: true },
      scrollbar: {
        vertical: "auto",
        horizontal: "hidden",
        useShadows: false
      }
    });

    editor.onDidContentSizeChange(function (e) {
      self.scheduleHeightSync(elementId);
    });

    this.registerTabSnippets(editor);
    this.editors[elementId] = editor;
    this.registerFormatCommand(elementId, host);

    self.markHostReady(host);
    self.applyContentHeight(elementId, editor.getContentHeight());
    self.bindKeyboardShortcuts(elementId);

    await new Promise(function (resolve) {
      requestAnimationFrame(function () {
        self.applyContentHeight(elementId, editor.getContentHeight());
        self.markHostReady(host);
        self.bindKeyboardShortcuts(elementId);
        if (window.gf2ExercisePage && window.gf2ExercisePage.checkReady) {
          window.gf2ExercisePage.checkReady();
        }
        resolve();
      });
    });
  },

  hostHasEditor: function (host) {
    return !!host && !!host.querySelector(".monaco-editor");
  },

  isVisible: function (host) {
    if (!host || !host.isConnected) return false;
    var rect = host.getBoundingClientRect();
    return rect.width >= 48 && rect.bottom > -40 && rect.top < window.innerHeight + 200;
  },

  initWhenVisible: async function (elementId, initialCode, options) {
    var host = document.getElementById(elementId);
    if (!host) return;
    if (this.isReady(elementId) && this.hostHasEditor(host)) return;

    options = options || {};
    var self = this;

    if (this.isVisible(host)) {
      await this.init(elementId, initialCode, options);
      return;
    }

    if (typeof IntersectionObserver === "undefined") {
      await this.init(elementId, initialCode, options);
      return;
    }

    this.detachLazyObserver(elementId);

    await new Promise(function (resolve) {
      var io = new IntersectionObserver(
        function (entries) {
          var entry = entries[0];
          if (!entry || !entry.isIntersecting) return;
          self.detachLazyObserver(elementId);
          self.init(elementId, initialCode, options).then(resolve);
        },
        { rootMargin: "200px 0px", threshold: 0.01 }
      );
      io.observe(host);
      self.lazyObservers[elementId] = io;
    });
  },

  disposeStale: function () {
    var self = this;
    var ids = Object.keys(this.editors);
    for (var i = 0; i < ids.length; i++) {
      var id = ids[i];
      var host = document.getElementById(id);
      if (!host || !document.body.contains(host)) {
        self.dispose(id);
      }
    }

    var lazyIds = Object.keys(this.lazyObservers);
    for (var j = 0; j < lazyIds.length; j++) {
      var lazyId = lazyIds[j];
      var lazyHost = document.getElementById(lazyId);
      if (!lazyHost || !document.body.contains(lazyHost)) {
        self.detachLazyObserver(lazyId);
      }
    }
  },

  decodePendingCode: function (encoded) {
    if (!encoded) return "";
    try {
      var binary = atob(encoded);
      var bytes = new Uint8Array(binary.length);
      for (var i = 0; i < binary.length; i++) bytes[i] = binary.charCodeAt(i);
      return new TextDecoder("utf-8").decode(bytes);
    } catch (e) {
      return "";
    }
  },

  ensureMounted: function () {
    var self = this;
    document.querySelectorAll(".playground-editor-host").forEach(function (host) {
      if (!host.id || self.hostHasEditor(host)) return;
      var code = self.decodePendingCode(host.dataset.pendingCode);
      if (code) self.initWhenVisible(host.id, code);
    });
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
    this.detachLazyObserver(elementId);
    delete this.heightSyncScheduled[elementId];
    delete this.ctrlRunHelpers[elementId];
    delete this.ctrlSaveHelpers[elementId];
    delete this.keyboardBound[elementId];

    var editor = this.editors[elementId];
    if (editor) {
      editor.dispose();
      delete this.editors[elementId];
    }

    delete this.editorOptions[elementId];

    var host = document.getElementById(elementId);
    if (host) {
      host.textContent = "";
      host.style.height = "";
      host.classList.remove("playground-editor-host-loading");
      delete host.dataset.editorReady;
    }
  },

  disposeAll: function () {
    var ids = Object.keys(this.editors);
    for (var i = 0; i < ids.length; i++) {
      this.dispose(ids[i]);
    }
    var lazyIds = Object.keys(this.lazyObservers);
    for (var j = 0; j < lazyIds.length; j++) {
      this.detachLazyObserver(lazyIds[j]);
    }
  }
};
