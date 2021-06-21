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
  it("User registration", (done) => {
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

  it("User Login", (done) => {
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

  it("User delete", (done) => {
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
});
