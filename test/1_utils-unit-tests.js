const assert = require("chai").assert;
const app = require("../index.js");
const authUtils = require("../lib/authUtils");

describe("Credentials format", () => {
  it("Validation email format", () => {
    const validEmails = [
      "email@example.com",
      "firstname.lastname@example.com",
      "email@subdomain.example.com",
      "email@[123.123.123.123]",
      '"email"@example.com',
      "1234567890@example.com",
      "email@example-one.com",
      "email@example.name",
      "email@example.museum",
      "email@example.co.jp",
      "firstname-lastname@example.com",
    ];

    validEmails.forEach((email) => {
      assert.isTrue(authUtils.validateEmailFormat(email));
    });

    const invalidEmails = [
      "plainaddress",
      "#@%^%#$@#$@#.com",
      "@example.com",
      "Joe Smith <email@example.com>",
      "email.example.com",
      "email@example@example.com",
      ".email@example.com",
      "email.@example.com",
      "email..email@example.com",
      "email@example.com (Joe Smith)",
      "email@example",
      "email@111.222.333.44444",
      "email@example..com",
      "Abc..123@example.com",
    ];

    invalidEmails.forEach((email) => {
      assert.isFalse(authUtils.validateEmailFormat(email));
    });
  });
});
