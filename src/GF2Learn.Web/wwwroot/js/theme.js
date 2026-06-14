(function (global) {
  var STORAGE_KEY = "gf2-theme";

  function systemPreference() {
    return global.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
  }

  function isValidMode(mode) {
    return mode === "light" || mode === "dark" || mode === "dos";
  }

  function readMode() {
    try {
      var stored = localStorage.getItem(STORAGE_KEY);
      if (isValidMode(stored)) return stored;
      if (stored === "system") {
        var migrated = systemPreference();
        localStorage.setItem(STORAGE_KEY, migrated);
        return migrated;
      }
    } catch { /* private browsing */ }
    return systemPreference();
  }

  function pathBase() {
    var meta = document.querySelector('meta[name="gf2-path-base"]');
    var base = meta ? meta.getAttribute("content") || "" : "";
    if (base && base.charAt(0) !== "/") base = "/" + base;
    if (base.endsWith("/")) base = base.slice(0, -1);
    return base;
  }

  function hljsThemeHref(mode) {
    var file = mode === "light" ? "github.min.css" : "github-dark.min.css";
    return pathBase() + "/vendor/highlightjs/styles/" + file;
  }

  function syncHljsStylesheet(mode) {
    var link = document.getElementById("hljs-theme");
    if (!link) return;
    var wanted = hljsThemeHref(mode);
    if (link.href.indexOf(wanted) === -1) {
      link.href = wanted;
    }
  }

  function apply(mode) {
    mode = mode || readMode();
    var root = document.documentElement;
    root.setAttribute("data-theme", mode);
    root.style.colorScheme = mode === "light" ? "light" : "dark";
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
    if (!isValidMode(mode)) return;
    try {
      localStorage.setItem(STORAGE_KEY, mode);
    } catch { /* private browsing */ }
    apply(mode);
  }

  function toggle() {
    setMode(readMode() === "light" ? "dark" : "light");
  }

  function updateToggleUi(mode) {
    document.querySelectorAll("[data-theme-toggle]").forEach(function (btn) {
      var isDark = mode !== "light";
      btn.classList.toggle("is-dark", isDark);
      btn.classList.toggle("is-dos", mode === "dos");
      btn.setAttribute("aria-checked", isDark ? "true" : "false");
    });

    document.querySelectorAll("[data-theme-option]").forEach(function (btn) {
      var selected = btn.dataset.themeOption === mode;
      btn.classList.toggle("is-selected", selected);
      btn.setAttribute("aria-pressed", selected ? "true" : "false");
    });

    document.querySelectorAll("[data-theme-status]").forEach(function (status) {
      var names = { light: "Lys", dark: "Mørk", dos: "DOS" };
      status.textContent = "Aktivt design: " + names[mode];
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

  function wireThemeOptions() {
    document.querySelectorAll("[data-theme-option]").forEach(function (btn) {
      if (btn.dataset.themeOptionWired) return;
      btn.dataset.themeOptionWired = "1";
      btn.addEventListener("click", function () {
        setMode(btn.dataset.themeOption);
      });
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
    wireThemeOptions();
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
    wireThemeOptions();
  });
})(window);
