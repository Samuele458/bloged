const chaiHttp = require("chai-http");
const chai = require("chai");
const assert = chai.assert;
const server = require("../index");
const { request } = require("chai");

chai.use(chaiHttp);

//variable used authorization
let token;

//waiting for mongoDB connection
before(function (done) {
  this.timeout(50000);
  console.log("Waiting for mongoDB connection...");
  server.on("ready", () => {
    done();
  });
});

describe("REST API tests", () => {
  describe("Users authentication", () => {
    it("POST /users/register - Valid credentials", (done) => {
      chai
        .request(server)
        .post("/users/register")
        .send({
          email: "exam.ple281636@example.com",
          username: "SampleUser23",
          password: "c0mmon_pw123@",
        })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.equal(res.body.success, true);
          done();
        });
    });

    it("POST /users/register - Wrong email format", (done) => {
      chai
        .request(server)
        .post("/users/register")
        .send({
          email: "exam.ple281636example.com",
          username: "SampleUser23",
          password: "c0mmon_pw123@",
        })
        .end((err, res) => {
          assert.equal(res.status, 400);
          assert.equal(res.body.success, false);
          done();
        });
    });

    it("POST /users/register - Wrong password format", (done) => {
      chai
        .request(server)
        .post("/users/register")
        .send({
          email: "exam.ple281636@example.com",
          username: "SampleUser23",
          password: "c",
        })
        .end((err, res) => {
          assert.equal(res.status, 400);
          assert.equal(res.body.success, false);
          done();
        });
    });

    it("POST /users/register - Wrong username format", (done) => {
      chai
        .request(server)
        .post("/users/register")
        .send({
          email: "exam.ple281636@example.com",
          username: "John^^",
          password: "c2434jhd",
        })
        .end((err, res) => {
          assert.equal(res.status, 400);
          assert.equal(res.body.success, false);
          done();
        });
    });

    it("POST /users/register - Username already exists", (done) => {
      chai
        .request(server)
        .post("/users/register")
        .send({
          email: "exam.ple281636@example.com",
          username: "SampleUser23",
          password: "c2434jhd",
        })
        .end((err, res) => {
          assert.equal(res.status, 400);
          assert.equal(res.body.success, false);
          done();
        });
    });

    it("POST /users/login - Valid credentials", (done) => {
      chai
        .request(server)
        .post("/users/login")
        .send({
          username: "SampleUser23",
          password: "c0mmon_pw123@",
        })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.equal(res.body.success, true);
          token = res.body.token;
          done();
        });
    });

    it("DELETE /users/:username - Trying to delete an unknown user without authorization", (done) => {
      chai
        .request(server)
        .delete("/users/randomUser382")
        .end((err, res) => {
          assert.equal(res.status, 401);
          done();
        });
    });

    it("DELETE /users/:username - Trying to delete an unknown user with authorization", (done) => {
      chai
        .request(server)
        .delete("/users/randomUser382")
        .set({ Authorization: token })
        .end((err, res) => {
          assert.equal(res.status, 401);
          assert.equal(res.body.success, false);
          done();
        });
    });

    it("DELETE /users/:username - Delete the owned user", (done) => {
      chai
        .request(server)
        .delete("/users/SampleUser23")
        .set({ Authorization: token })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.equal(res.body.success, true);
          done();
        });
    });
  });
});
