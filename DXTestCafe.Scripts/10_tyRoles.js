import { Role } from "testcafe";

export const adminUser = Role("https://localhost:44382/Identity/Account/Login", async t => {
    await t
        .typeText("#Input_Email", "admin@localhost")
        .typeText("#Input_Password", "Test123$")
        .click("#login-submit");
});

export const regularUser = Role("https://localhost:44382/Identity/Account/Login", async t => {
    await t
        .typeText("#Input_Email", "don@localhost")
        .typeText("#Input_Password", "Test456&")
        .click("#login-submit");
});