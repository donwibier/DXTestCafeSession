import { Selector } from "testcafe";

fixture("My Thougths")
    .page("https://localhost:44382/Identity/Account/Login");

    test("User with valid account can log in", async t => {
        await t
            .typeText("#Input_Email", "admin@localhost")
            .typeText("#Input_Password", "Test123$");

        await t.click("#login-submit");
        
        await t
            .expect(Selector("#loggedin-user").innerText)
            .contains("admin@localhost");

});