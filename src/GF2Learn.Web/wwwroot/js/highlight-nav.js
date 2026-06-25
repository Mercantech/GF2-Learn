(function (global) {
  function pathBase() {
    var meta = document.querySelector('meta[name="gf2-path-base"]');
    var base = meta ? meta.getAttribute("content") || "" : "";
    if (base && base.charAt(0) !== "/") base = "/" + base;
    if (base.endsWith("/")) base = base.slice(0, -1);
    return base;
  }

  function curriculumPrefix() {
    return pathBase() + "/curriculum";
  }

  function isCurriculumPath(pathname) {
    var prefix = curriculumPrefix();
    return pathname === prefix || pathname.startsWith(prefix + "/");
  }

  /** Pensum bruger WASM + Monaco — fuld sideindlæsning er mere pålidelig end enhanced navigation. */
  function forceFullLoadForCurriculumLinks() {
    document.addEventListener(
      "click",
      function (e) {
        if (e.defaultPrevented) return;
        if (e.button !== 0 || e.metaKey || e.ctrlKey || e.shiftKey || e.altKey) return;

        var anchor = e.target.closest("a[href]");
        if (!anchor || anchor.hasAttribute("download")) return;
        if (anchor.target && anchor.target !== "_self") return;
        if (anchor.getAttribute("data-enhance-nav") === "false") return;

        var href = anchor.getAttribute("href");
        if (!href || href.charAt(0) === "#" || href.indexOf("javascript:") === 0) return;

        var url;
        try {
          url = new URL(href, global.location.href);
        } catch (err) {
          return;
        }

        if (url.origin !== global.location.origin) return;
        if (!isCurriculumPath(url.pathname)) return;

        if (
          url.pathname === global.location.pathname &&
          url.search === global.location.search &&
          url.hash
        ) {
          return;
        }

        e.preventDefault();
        e.stopPropagation();
        global.location.assign(url.pathname + url.search + url.hash);
      },
      true
    );
  }

  /** Fjern kun Monaco-instanser hvis DOM-host er væk (undgå at slette nye editorer efter routing). */
  function prepareNavigation() {
    if (global.gf2Playground && global.gf2Playground.disposeStale) {
      global.gf2Playground.disposeStale();
    }
    if (global.gf2Highlight && global.gf2Highlight.unwrapCodeBlocks) {
      global.gf2Highlight.unwrapCodeBlocks(document);
    }
  }

  /** Kør EFTER DOM er opdateret — må ikke dispose Monaco (det ødelægger nye editorer). */
  function enhancePage() {
    requestAnimationFrame(function () {
      requestAnimationFrame(function () {
        if (global.gf2Theme) global.gf2Theme.apply(global.gf2Theme.getMode());
        global.dispatchEvent(new CustomEvent("gf2-enhanced-nav"));
        if (global.gf2Highlight) global.gf2Highlight.process(document);
        if (global.gf2Playground && global.gf2Playground.ensureMountedWithRetries) {
          global.gf2Playground.ensureMountedWithRetries();
        } else if (global.gf2Playground && global.gf2Playground.ensureMounted) {
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
    forceFullLoadForCurriculumLinks();
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
