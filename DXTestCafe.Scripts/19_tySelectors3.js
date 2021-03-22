/*
Example HTML fragment:
<div class="feed-posts">First post</div>
<div class="feed-posts">Second post</div>
<div class="feed-posts">Third post</div>
*/

// Targets `<div class="feed-posts">First post</div>`
Selector(".feed-posts").withText("First");

// Targets `<div class="feed-posts">Third post</div>`
// (this is a case-insensitive regular expression)
Selector(".feed-posts").withText(/third/i);

// Returns a NodeList of all divs
Selector(".feed-posts").withText("post");