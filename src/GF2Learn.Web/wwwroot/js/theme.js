(function (global) {
  var STORAGE_KEY = "gf2-theme";

  function systemPreference() {
    return global.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
  }

  function readMode() {
    try {
      var stored = localStorage.getItem(STORAGE_KEY);
      if (stored === "light" || stored === "dark") return stored;
      if (stored === "system") {
        var migrated = systemPreference();
        localStorage.setItem(STORAGE_KEY, migrated);
        return migrated;
      }
    } catch { /* private browsing */ }
    return systemPreference();
  }

  function syncHljsStylesheet(mode) {
    var link = document.getElementById("hljs-theme");
    if (!link) return;
    var dark = mode === "dark";
    var wanted = dark
      ? "https://cdn.jsdelivr.net/npm/highlight.js@11.11.1/styles/github-dark.min.css"
      : "https://cdn.jsdelivr.net/npm/highlight.js@11.11.1/styles/github.min.css";
    if (!link.href.endsWith(dark ? "github-dark.min.css" : "github.min.css")) {
      link.href = wanted;
    }
  }

  function apply(mode) {
    mode = mode || readMode();
    var root = document.documentElement;
    root.setAttribute("data-theme", mode);
    root.style.colorScheme = mode;
    updateToggleUi(mode);
    syncHljsStylesheet(mode);
    if (global.gf2Playground && global.gf2Playground.applyTheme) {
      global.gf2Playground.applyTheme();
    }
    if (global.gf2Highlight && global.gf2Highlight.syncTheme) {
      global.gf2Highlight.syncTheme();
    }
    global.dispatchEvent(
      new CustomEvent("gf2-theme-change", { detail: { mode: mode, effective: mode } })
    );
  }

  function setMode(mode) {
    if (mode !== "light" && mode !== "dark") return;
    try {
      localStorage.setItem(STORAGE_KEY, mode);
    } catch { /* private browsing */ }
    apply(mode);
  }

  function toggle() {
    setMode(readMode() === "dark" ? "light" : "dark");
  }

  function updateToggleUi(mode) {
    document.querySelectorAll("[data-theme-toggle]").forEach(function (btn) {
      var isDark = mode === "dark";
      btn.classList.toggle("is-dark", isDark);
      btn.setAttribute("aria-checked", isDark ? "true" : "false");
    });
  }

  function wireToggles() {
    document.querySelectorAll("[data-theme-toggle]").forEach(function (btn) {
      if (btn.dataset.themeWired) return;
      btn.dataset.themeWired = "1";
      btn.addEventListener("click", toggle);
    });
    updateToggleUi(readMode());
  }

  global.gf2Theme = {
    getMode: readMode,
    setMode: setMode,
    toggle: toggle,
    getEffective: readMode,
    apply: apply
  };

  function onReady() {
    wireToggles();
    updateToggleUi(readMode());
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", onReady);
  } else {
    onReady();
  }

  document.addEventListener("gf2-enhanced-nav", function () {
    apply(readMode());
    wireToggles();
  });
})(window);
