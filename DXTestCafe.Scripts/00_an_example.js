import { Selector } from "testcafe"; 

fixture("TestCafe Example")
    .page("http://devexpress.github.io/testcafe/example");

    test("Fill out and submit form", async t => {
        // Fill out the "Your name" field.
        
        await t.typeText("#developer-name", "John Doe");

        // Select the first three checkboxes under the "Which features are 
        // important to you" section.
        await t
            .click("#remote-testing") 
            .click("#reusing-js-code") 
            .click("#background-parallel-testing");

        // Select the MacOS option in the "What is your primary Operating 
        // System" section.
        await t.click("#macos");

        // Select the JavaScript API option under the dropdown list in the 
        // "Which TestCafe interface do you use" section.
        const preferredInterface = Selector("#preferred-interface"); 
        await t.click(preferredInterface) .click(preferredInterface.find("option").withText("JavaScript API"));

        // Verify that the Submit button is enabled.
        const submitButton = Selector("#submit-button"); 
        await t.expect(submitButton.hasAttribute("disabled")).notOk(); 

        // Submit the form by clicking the Submit button.
        await t.click(submitButton);
        
        // Validate the form submission by checking the content that appears 
        // on the page.
        const headerInfo = Selector("#article-header");
        await t.expect(headerInfo.innerText).eql("Thank you, John Doe!");
    });

