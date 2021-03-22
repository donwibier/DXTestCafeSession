fixture("Fixture and test hooks")
    .before(async ctx => {
    // The code inside this function runs only
    // once, before the beforeEach test hook.
    })

    .beforeEach(async t => {
    // The code inside this function runs after
    // the before function and before each test.
    })

    .afterEach(async t => {
    // The code inside this function runs before
    // the after function and after each test.
    })
    
    .after(async ctx => {
    // The code inside this function runs only
    // once, after the afterEach test hook.
    });