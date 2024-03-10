import { DotNet } from '@microsoft/dotnet-js-interop';

let scrollContainer: HTMLElement = null;
const observersByDotNetId = {};

const findClosestScrollContainer = (element: HTMLElement | null): HTMLElement | null => {
  if (!element || element === document.body || element === document.documentElement) {
    return null;
  }

  const style = getComputedStyle(element);

  if (style.overflowY !== 'visible') {
    return element;
  }

  return findClosestScrollContainer(element.parentElement);
}

const dispatcherObserversByDotNetIdPropname = Symbol();

const getObserversMapEntry = (dotNetHelper: DotNet.DotNetObject): { observersByDotNetObjectId: { [id: number]: any }, id: number } => {
  const dotNetHelperId = dotNetHelper['_id'];
  const dotNetHelperDispatcher = dotNetHelper['_callDispatcher'];
  // .Net8
  if (dotNetHelperDispatcher) {
    dotNetHelperDispatcher[dispatcherObserversByDotNetIdPropname] ??= {};
    return {
      observersByDotNetObjectId: dotNetHelperDispatcher[dispatcherObserversByDotNetIdPropname],
      id: dotNetHelperId,
    };
  }
  // dotnet less than .Net8
  return {
    observersByDotNetObjectId: observersByDotNetId,
    id: dotNetHelperId,
  };
}

const invokeInit = (dotNetHelper: DotNet.DotNetObject, container: HTMLElement): void => {
  const contentWidth = container.clientWidth;
  dotNetHelper.invokeMethodAsync("OnContentWidthChange", contentWidth, true);
}

const init = (dotNetHelper: DotNet.DotNetObject, spacerBefore: HTMLElement, spacerAfter: HTMLElement) => {
  const container = spacerBefore.parentElement;
  invokeInit(dotNetHelper, container);
  scrollContainer = findClosestScrollContainer(container);
  const intersectionObserver = new IntersectionObserver(intersectionCallback, {
    root: scrollContainer,
    rootMargin: `50px`,
  });

  intersectionObserver.observe(spacerBefore);
  intersectionObserver.observe(spacerAfter);
  const mutationObserverBefore = createSpacerMutationObserver(spacerBefore);
  const mutationObserverAfter = createSpacerMutationObserver(spacerAfter);
  const resizeObserver = new ResizeObserver(resizeCallback);
  resizeObserver.observe(container);
  const { observersByDotNetObjectId, id } = getObserversMapEntry(dotNetHelper);
  observersByDotNetObjectId[id] = {
    intersectionObserver,
    mutationObserverBefore,
    mutationObserverAfter,
    resizeObserver,
  };

  function createSpacerMutationObserver(spacer: Element): MutationObserver {
    const observerOptions = { attributes: true };
    const mutationObserver = new MutationObserver((mutations: MutationRecord[], observer: MutationObserver): void => {
      intersectionObserver.unobserve(spacer);
      intersectionObserver.observe(spacer);
    });

    mutationObserver.observe(spacer, observerOptions);
    return mutationObserver;
  }

  function intersectionCallback(entries: IntersectionObserverEntry[]): void {
    entries.forEach((entry): void => {
      if (!entry.isIntersecting) {
        return;
      }
      const containerHeight = (scrollContainer || document.documentElement).clientHeight;
      const scrollTop = (scrollContainer || document.documentElement).scrollTop;
      const scrollHeight = (scrollContainer || document.documentElement).scrollHeight;

      if (entry.target == spacerBefore) {
        dotNetHelper.invokeMethodAsync('OnSpacerBeforeVisible', scrollTop, scrollHeight, containerHeight);
      } else if (entry.target == spacerAfter) {
        const style = getComputedStyle(spacerAfter);
        if (parseInt(style.top.replace('px', '')) > 0) {
          dotNetHelper.invokeMethodAsync('OnSpacerAfterVisible', scrollTop, scrollHeight, containerHeight);
        }
      }
    });
  }

  function resizeCallback(entries: ResizeObserverEntry[]): void {
    entries.forEach((entry): void => {
      if (entry.target == container) {
        dotNetHelper.invokeMethodAsync("OnContentWidthChange", entry.contentRect.width, false);
      }
    });
  }
}

const scrollTo = (top: number): void => {
  if (!scrollContainer) {
    scrollContainer.scrollTo({
      top: top,
      behavior: 'smooth',
    });
  }
}

const dispose = (dotNetHelper: DotNet.DotNetObject): void => {
  const { observersByDotNetObjectId, id } = getObserversMapEntry(dotNetHelper);
  const observers = observersByDotNetObjectId[id];

  if (observers) {
    observers.intersectionObserver.disconnect();
    observers.mutationObserverBefore.disconnect();
    observers.mutationObserverAfter.disconnect();
    observers.resizeObserver.disconnect();

    dotNetHelper.dispose();
    delete observersByDotNetObjectId[id];
  }
}

export const VirtualList = {
  init,
  scrollTo,
  dispose,
}

window['VirtualList'] = VirtualList
