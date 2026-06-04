/* Synkron i <head> før CSS — undgår flash til lys tema */
(function () {
  var STORAGE_KEY = "gf2-theme";
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
  hljs.href =
    mode === "dark"
      ? "https://cdn.jsdelivr.net/npm/highlight.js@11.11.1/styles/github-dark.min.css"
      : "https://cdn.jsdelivr.net/npm/highlight.js@11.11.1/styles/github.min.css";
  document.head.appendChild(hljs);
})();
