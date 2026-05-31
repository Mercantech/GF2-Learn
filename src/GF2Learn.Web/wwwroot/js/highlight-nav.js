(function (global) {
  function onPageUpdated() {
    if (global.gf2Playground && global.gf2Playground.disposeAll) {
      global.gf2Playground.disposeAll();
    }
    requestAnimationFrame(function () {
      requestAnimationFrame(function () {
        if (global.gf2Theme) global.gf2Theme.apply(global.gf2Theme.getMode());
        global.dispatchEvent(new CustomEvent("gf2-enhanced-nav"));
        if (global.gf2Highlight) global.gf2Highlight.process(document);
      });
    });
  }

  function wire() {
    if (global.Blazor && global.Blazor.addEventListener) {
      global.Blazor.addEventListener("enhancedload", onPageUpdated);
    }
  }

  wire();
})(window);
