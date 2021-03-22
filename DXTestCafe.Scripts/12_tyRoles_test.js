import { Selector } from "testcafe";
import { adminUser, regularUser } from "./10_tyRoles";

fixture("My Thoughts switch accounts")
    .page("https://localhost:44382/");

    test("admin can see admin sections on the sidebar", async t => {
        await t.useRole(adminUser);
        // check your admin tests
        // await t
        //     .expect(Selector("some-selector").innerText).Contains(...)
        });

    test("user can't see admin sections on the sidebar", async t => {
        await t.useRole(regularUser);
        // check your regular tests
        // await t
        //     .expect(Selector("some-selector").innerText).notContains("...");
        });

    test("Anonymous role test", async t => {
        // Logs in the administrator account.
        await t.useRole(adminUser);
        // Perform actions / assertions as administrator.
        // "Logs out" the administrator user.
        await t.useRole(Role.anonymous());
        // Perform actions / assertions as a logged-out user.
    });