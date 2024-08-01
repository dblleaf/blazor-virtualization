const observersByDotNetId = {};
const findClosestScrollContainer = (element) => {
    if (!element || element === document.body || element === document.documentElement) {
        return null;
    }
    const style = getComputedStyle(element);
    if (style.overflowY !== 'visible') {
        return element;
    }
    return findClosestScrollContainer(element.parentElement);
};
const dispatcherObserversByDotNetIdPropname = Symbol();
const getObserversMapEntry = (dotNetHelper) => {
    var _a;
    const dotNetHelperId = dotNetHelper['_id'];
    const dotNetHelperDispatcher = dotNetHelper['_callDispatcher'];
    if (dotNetHelperDispatcher) {
        (_a = dotNetHelperDispatcher[dispatcherObserversByDotNetIdPropname]) !== null && _a !== void 0 ? _a : (dotNetHelperDispatcher[dispatcherObserversByDotNetIdPropname] = {});
        return {
            observersByDotNetObjectId: dotNetHelperDispatcher[dispatcherObserversByDotNetIdPropname],
            id: dotNetHelperId,
        };
    }
    return {
        observersByDotNetObjectId: observersByDotNetId,
        id: dotNetHelperId,
    };
};
const invokeInit = (dotNetHelper, container) => {
    const contentWidth = container.clientWidth;
    dotNetHelper.invokeMethodAsync("OnContentWidthChange", contentWidth, true);
};
const init = (dotNetHelper, spacerBefore, spacerAfter) => {
    const container = spacerBefore.parentElement;
    invokeInit(dotNetHelper, container);
    const scrollContainer = findClosestScrollContainer(container);
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
        scrollContainer,
    };
    function createSpacerMutationObserver(spacer) {
        const observerOptions = { attributes: true };
        const mutationObserver = new MutationObserver((mutations, observer) => {
            intersectionObserver.unobserve(spacer);
            intersectionObserver.observe(spacer);
        });
        mutationObserver.observe(spacer, observerOptions);
        return mutationObserver;
    }
    function intersectionCallback(entries) {
        entries.forEach((entry) => {
            if (!entry.isIntersecting) {
                return;
            }
            const containerHeight = (scrollContainer || document.documentElement).clientHeight;
            const scrollTop = (scrollContainer || document.documentElement).scrollTop;
            const scrollHeight = (scrollContainer || document.documentElement).scrollHeight;
            if (entry.target == spacerBefore) {
                dotNetHelper.invokeMethodAsync('OnSpacerBeforeVisible', scrollTop, containerHeight);
            }
            else if (entry.target == spacerAfter) {
                const style = getComputedStyle(spacerAfter);
                if (parseInt(style.top.replace('px', '')) > 0) {
                    dotNetHelper.invokeMethodAsync('OnSpacerAfterVisible', scrollTop, scrollHeight, containerHeight);
                }
            }
        });
    }
    function resizeCallback(entries) {
        entries.forEach((entry) => {
            if (entry.target == container) {
                dotNetHelper.invokeMethodAsync("OnContentWidthChange", entry.contentRect.width, false);
            }
        });
    }
};
const scrollTo = (dotNetHelper, top) => {
    const { observersByDotNetObjectId, id } = getObserversMapEntry(dotNetHelper);
    const observers = observersByDotNetObjectId[id];
    const scrollContainer = observers.scrollContainer || document.documentElement;
    if (scrollContainer) {
        scrollContainer.scrollTo({
            top: top,
            behavior: 'smooth',
        });
    }
};
const dispose = (dotNetHelper) => {
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
};
export const VirtualList = {
    init,
    scrollTo,
    dispose,
};
window['VirtualList'] = VirtualList;
//# sourceMappingURL=VirtualList.js.map