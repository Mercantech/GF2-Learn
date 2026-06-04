(function (global) {
  /** Kør FØR ny side renderes (rydder gamle Monaco-instanser). */
  function prepareNavigation() {
    if (global.gf2Playground && global.gf2Playground.disposeAll) {
      global.gf2Playground.disposeAll();
    }
  }

  /** Kør EFTER DOM er opdateret — må ikke dispose Monaco (det ødelægger nye editorer). */
  function enhancePage() {
    requestAnimationFrame(function () {
      requestAnimationFrame(function () {
        if (global.gf2Theme) global.gf2Theme.apply(global.gf2Theme.getMode());
        global.dispatchEvent(new CustomEvent("gf2-enhanced-nav"));
        if (global.gf2Highlight) global.gf2Highlight.process(document);
        if (global.gf2Playground && global.gf2Playground.layoutVisibleDelayed) {
          global.gf2Playground.layoutVisibleDelayed();
        }
      });
    });
  }

  function onEnhancedLoad() {
    prepareNavigation();
    enhancePage();
  }

  function wire() {
    if (global.Blazor && global.Blazor.addEventListener) {
      global.Blazor.addEventListener("enhancedload", onEnhancedLoad);
    }
  }

  global.gf2Nav = global.gf2Nav || {};
  global.gf2Nav.prepareNavigation = prepareNavigation;
  global.gf2Nav.enhancePage = enhancePage;
  /** @deprecated Brug prepareNavigation + enhancePage */
  global.gf2Nav.pageUpdated = enhancePage;

  wire();
})(window);
