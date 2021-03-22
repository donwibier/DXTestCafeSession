fixture("Fixture using test context")
    .beforeEach(async t => {
        // Adding the property 'loginName' to the
        // test context object and setting a value.
        t.ctx.loginName = "Don Wibier"
    });

    test("Accessing test context", async t => {
        // Read the 'loginName' property set in the
        // beforeEach function for the fixture.
        await t
            .expect(t.ctx.loginName)
            .eql("Don Wibier");
    });

