

// Finds all elements with the class name of "feed-posts".
const feedPosts = Selector(".feed-posts");



// Also finds all elements with the class name of
// "feed-posts", but overrides its default options.
const visibleFeedPosts = Selector(feedPosts, {
        visibilityCheck: true
    });




    const myElement = Selector(() => {
        // Fetches the string located in the 'elementId'
        // key in the browser's local storage.
        const localStorageId = window.localStorage.elementId;
        // Returns the element's node idenfied by the ID.
        return document.getElementById(localStorageId);
    });