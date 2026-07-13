(function () {
  function enhancePlaygrounds() {
    document.querySelectorAll(".code-playground:not(.code-playground-interactive)").forEach(function (el) {
      if (el.dataset.enhanced) return;
      el.dataset.enhanced = "1";
      var code = el.getAttribute("data-code") || "";
      var expected = el.getAttribute("data-expected") || "";
      var pre = el.querySelector(".playground-code");
      if (pre) pre.textContent = code;

      var toolbar = document.createElement("div");
      toolbar.className = "playground-toolbar";

      var copyBtn = document.createElement("button");
      copyBtn.type = "button";
      copyBtn.textContent = "Kopier kode";
      copyBtn.addEventListener("click", function () {
        navigator.clipboard.writeText(code).then(function () {
          copyBtn.textContent = "Kopieret!";
          setTimeout(function () { copyBtn.textContent = "Kopier kode"; }, 1500);
        });
      });

      toolbar.appendChild(copyBtn);
      el.appendChild(toolbar);

      if (expected) {
        var out = document.createElement("div");
        out.className = "playground-expected";
        out.textContent = "Forventet output: " + expected;
        el.appendChild(out);
      }
    });
  }

  document.addEventListener("DOMContentLoaded", enhancePlaygrounds);
  document.addEventListener("gf2-enhanced-nav", enhancePlaygrounds);

  document.addEventListener("DOMContentLoaded", function () {
    if (window.gf2Highlight) window.gf2Highlight.process(document);
  });
})();

window.gf2ScrollToBottom = function (element) {
  if (!element) return;
  requestAnimationFrame(function () {
    element.scrollTop = element.scrollHeight;
  });
};
