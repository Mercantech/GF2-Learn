(function (global) {
  var lightTheme = "https://cdn.jsdelivr.net/npm/highlight.js@11.11.1/styles/github.min.css";
  var darkTheme = "https://cdn.jsdelivr.net/npm/highlight.js@11.11.1/styles/github-dark.min.css";

  function effectiveDarkMode() {
    var mode = document.documentElement.getAttribute("data-theme");
    if (mode === "dark") return true;
    if (mode === "light") return false;
    return global.matchMedia && global.matchMedia("(prefers-color-scheme: dark)").matches;
  }

  function hljsThemeUrl() {
    return effectiveDarkMode() ? darkTheme : lightTheme;
  }

  function syncHljsTheme() {
    var link = document.getElementById("hljs-theme");
    if (link) link.href = hljsThemeUrl();
  }

  function langLabel(code) {
    var match = (code.className || "").match(/language-([\w+#]+)/);
    if (!match) return "code";
    var map = { csharp: "C#", cs: "C#", bash: "Terminal", shell: "Terminal", json: "JSON" };
    return map[match[1].toLowerCase()] || match[1].toUpperCase();
  }

  function enhanceCodeBlocks(root) {
    if (!root) return;
    root.querySelectorAll("pre > code").forEach(function (code) {
      var pre = code.parentElement;
      if (!pre || pre.dataset.codeEnhanced) return;
      if (pre.closest(".code-playground")) return;
      if (pre.closest(".landing-code-window")) return;
      pre.dataset.codeEnhanced = "1";

      var lang = langLabel(code);
      pre.setAttribute("data-lang", lang);

      var shell = document.createElement("div");
      shell.className = "code-block";
      pre.parentNode.insertBefore(shell, pre);
      shell.appendChild(pre);

      var bar = document.createElement("div");
      bar.className = "code-block-bar";
      bar.innerHTML =
        '<span class="code-block-dots" aria-hidden="true">' +
        '<span></span><span></span><span></span></span>' +
        '<span class="code-block-lang">' + lang + "</span>";

      var copyBtn = document.createElement("button");
      copyBtn.type = "button";
      copyBtn.className = "code-block-copy";
      copyBtn.textContent = "Kopier";
      copyBtn.addEventListener("click", function () {
        navigator.clipboard.writeText(code.textContent || "").then(function () {
          copyBtn.textContent = "Kopieret!";
          setTimeout(function () { copyBtn.textContent = "Kopier"; }, 1500);
        });
      });
      bar.appendChild(copyBtn);
      shell.insertBefore(bar, pre);
    });
  }

  function detectLanguage(code) {
    var match = (code.className || "").match(/language-([\w+#-]+)/i);
    return match ? match[1].toLowerCase() : null;
  }

  function codesInRoot(root) {
    if (!root) return [];
    if (root.tagName === "CODE") return [root];
    if (root.tagName === "PRE") {
      var direct = root.querySelector(":scope > code");
      return direct ? [direct] : [];
    }
    if (!root.querySelectorAll) return [];
    return Array.prototype.slice.call(
      root.querySelectorAll("pre > code, code.language-csharp, code.language-cs")
    );
  }

  function highlightCode(code) {
    var hljs = global.hljs;
    if (!hljs) return false;
    if (code.closest(".landing-code-window")) return false;

    code.classList.remove("hljs");
    code.removeAttribute("data-highlighted");

    var lang = detectLanguage(code);
    if (lang === "cs") lang = "csharp";
    if (!lang && code.closest(".curriculum-content, .markdown-body, .code-playground, .runnable-code-display")) {
      lang = "csharp";
    }

    try {
      if (lang && hljs.getLanguage(lang)) {
        var result = hljs.highlight(code.textContent, { language: lang, ignoreIllegals: true });
        code.innerHTML = result.value;
        code.classList.add("hljs");
        code.dataset.highlighted = "yes";
        return true;
      }
      hljs.highlightElement(code);
      return code.classList.contains("hljs") || code.dataset.highlighted === "yes";
    } catch (e) {
      console.warn("Syntax highlight failed:", lang || "auto", e);
      return false;
    }
  }

  function highlightAll(root) {
    if (!global.hljs) return false;
    var scope = root && root.querySelectorAll ? root : document;
    var ok = false;
    scope.querySelectorAll("pre > code").forEach(function (code) {
      if (highlightCode(code)) ok = true;
    });
    return ok;
  }

  function process(root) {
    root = root || document;
    syncHljsTheme();
    enhanceCodeBlocks(root);
    return highlightAll(root);
  }

  function scheduleProcess(root) {
    requestAnimationFrame(function () {
      process(root || document);
    });
  }

  function highlightRoot(root) {
    if (!root || !global.hljs) return false;
    syncHljsTheme();
    var nodes = codesInRoot(root);
    if (nodes.length === 0) return false;
    var ok = false;
    nodes.forEach(function (code) {
      if (highlightCode(code)) ok = true;
    });
    return ok;
  }

  function observeDynamicContent() {
    document.querySelectorAll(".curriculum-content, .markdown-body").forEach(function (watch) {
      if (watch.dataset.highlightObserved === "1") return;
      watch.dataset.highlightObserved = "1";
      var timer = null;
      new MutationObserver(function () {
        clearTimeout(timer);
        timer = setTimeout(function () {
          scheduleProcess(watch);
        }, 120);
      }).observe(watch, { childList: true, subtree: true });
    });
  }

  global.gf2Highlight = {
    process: process,
    processRoot: process,
    highlightRoot: highlightRoot,
    syncTheme: syncHljsTheme
  };

  global.addEventListener("gf2-theme-change", syncHljsTheme);

  function boot() {
    syncHljsTheme();
    observeDynamicContent();
    scheduleProcess(document);
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", boot);
  } else {
    boot();
  }

  global.addEventListener("gf2-enhanced-nav", function () {
    observeDynamicContent();
    scheduleProcess(document);
  });
})(window);
