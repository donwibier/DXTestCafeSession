fixture("Fixture with test hooks")
    .beforeEach(async t => {
        // The code inside this function runs
        // before every test in this fixture
        // except where overridden.
    })

    .afterEach(async t => {
        // The code inside this function runs
        // before every test in this fixture
        // except where overridden.
    });

    test
        .before(async t => {
            // The code inside this function runs
            // before this test, ignoring the code
            // defined in the beforeEach function
            // in the fixture.
        })

        .after(async t => {
            // The code inside this function runs
            // after this test, ignoring the code
            // defined in the afterEach function
            // in the fixture.
        })

        ("Test with test hooks", async t => {
            // This test only executes the code
            // defined in the before and after
            // functions in the test object.
        });
   
    test("Test without test hooks", async t => {
        // This test executes the code defined in
        // the beforeEach and afterEach functions
        // in the fixture.
    });

    test("Another test without test hooks", async t => {
        // This test also executes the code defined
        // in the beforeEach and afterEach functions
        // in the fixture.
    });