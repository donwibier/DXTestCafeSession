/*
Example HTML fragment:
<div class="feed-posts">First post</div>
<div class="feed-posts">Second post</div>
<div class="feed-posts">Third post</div>
*/

// Targets `<div class="feed-posts">First post</div>`
Selector(".feed-posts").withExactText("First post");

// No elements are targeted because there are no elements
// with the exact string 'first' (lower-case 'f')
Selector(".feed-posts").withExactText("first post");

// No elements are targeted because there are no elements
// with the exact string 'First'
Selector(".feed-posts").withExactText("First");
