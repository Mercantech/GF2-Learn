/* Synkron i <head> før CSS — undgår flash til lys tema */
(function () {
  var STORAGE_KEY = "gf2-theme";

  function pathBase() {
    var meta = document.querySelector('meta[name="gf2-path-base"]');
    var base = meta ? meta.getAttribute("content") || "" : "";
    if (base && base.charAt(0) !== "/") base = "/" + base;
    if (base.endsWith("/")) base = base.slice(0, -1);
    return base;
  }

  function hljsThemeHref(mode) {
    var file = mode === "dark" ? "github-dark.min.css" : "github.min.css";
    return pathBase() + "/vendor/highlightjs/styles/" + file;
  }

  var mode;
  try {
    var stored = localStorage.getItem(STORAGE_KEY);
    if (stored === "light" || stored === "dark") mode = stored;
  } catch (e) { /* private browsing */ }

  if (!mode) {
    mode = window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
  }

  var root = document.documentElement;
  root.setAttribute("data-theme", mode);
  root.style.colorScheme = mode;

  var hljs = document.createElement("link");
  hljs.id = "hljs-theme";
  hljs.rel = "stylesheet";
  hljs.href = hljsThemeHref(mode);
  document.head.appendChild(hljs);
})();
