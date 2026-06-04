(function (global) {
  /** Fjern kun Monaco-instanser hvis DOM-host er væk (undgå at slette nye editorer efter routing). */
  function prepareNavigation() {
    if (global.gf2Playground && global.gf2Playground.disposeStale) {
      global.gf2Playground.disposeStale();
    }
  }

  /** Kør EFTER DOM er opdateret — må ikke dispose Monaco (det ødelægger nye editorer). */
  function enhancePage() {
    requestAnimationFrame(function () {
      requestAnimationFrame(function () {
        if (global.gf2Theme) global.gf2Theme.apply(global.gf2Theme.getMode());
        global.dispatchEvent(new CustomEvent("gf2-enhanced-nav"));
        if (global.gf2Highlight) global.gf2Highlight.process(document);
        if (global.gf2Playground && global.gf2Playground.ensureMounted) {
          global.gf2Playground.ensureMounted();
        }
        if (global.gf2Playground && global.gf2Playground.refreshAll) {
          global.gf2Playground.refreshAll();
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
