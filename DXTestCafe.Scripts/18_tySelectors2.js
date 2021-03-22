/*
Example HTML fragment:
<div class="feed-posts">First post</div>
<div class="feed-posts">Second post</div>
<div class="feed-posts">Third post</div>
*/

// Targets `<div class="feed-posts">First post</div>`
Selector(".feed-posts").nth(0);

// Targets `<div class="feed-posts">Second post</div>`
Selector(".feed-posts").nth(1);

// Targets `<div class="feed-posts">Third post</div>`
Selector(".feed-posts").nth(2);

// No elements are targeted since there are only three
// elements matching the .feed-posts class.
Selector(".feed-posts").nth(3);
