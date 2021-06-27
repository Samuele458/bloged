require("dotenv").config();
const express = require("express");
const path = require("path");
const cors = require("cors");
const passport = require("passport");

const app = new express();

//MongoDB database
require("./config/database")(app);

//loading models
require("./models/user");
require("./models/blog");

//fetch POST parameters
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

//allow frontend to make requests to backend
app.use(cors());

//passport configuration
require("./config/passport");

//pass the global passport object into the configuration function
require("./config/passport")(passport);

//initialize the passport object on every request
app.use(passport.initialize());

//loading API routes
app.use(require("./routes"));

//error handler
app.use((err, req, res, next) => {
  if (err.statusCode) {
    console.log("Handling error:", err.message, err.statusCode);
    res.status(err.statusCode).json({ success: false, msg: err.message });
  }

  console.log("Error: ", err.message);
  console.log(err.stack);
  res.status(500).json({ success: false, msg: "Error" });
});

const port = process.env.PORT || 3000;
app.listen(port, () => {
  console.log(`App listening on port ${port}`);
});

module.exports = app;
