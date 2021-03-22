
    test.before(async t => {
        t.ctx.loginName = "New User";
        })
    
    ("Accessing test context", async t => {
        // This test passes because the test
        // context property is defined in the
        // test hook.
        await t
            .expect(t.ctx.loginName)
            .eql("New User");
        });

        test("Checking if context still works", async t => {
        // This test will fail because the test
        // context property does not define the
        // loginName property in the fixture of
        // test.
        await t
            .expect(t.ctx.loginName)
            .eql("New User");
    });