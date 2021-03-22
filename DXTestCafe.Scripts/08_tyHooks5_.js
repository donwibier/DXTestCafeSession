fixture("Using fixture context")
    .before(async ctx => {
        ctx.loginName = "Don Wibier"
    });

test("Accessing fixture context", async t => {
    // Read the 'loginName' property set in the
    // before function for the fixture.
    await t
        .expect(t.fixtureCtx.loginName)
        .eql("Don Wibier");
    });