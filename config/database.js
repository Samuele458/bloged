require("dotenv").config();
const mongoose = require("mongoose");

const prodUri = process.env.PROD_MONGO_URI;
const devUri = process.env.DEV_MONGO_URI;

let mongoUri = "";

if (process.env.NODE_ENV === "production") {
  mongoUri = prodUri;
} else {
  mongoUri = devUri;
}

mongoose.connect(mongoUri, {
  useNewUrlParser: true,
  useUnifiedTopology: true,
  useFindAndModify: false,
});

mongoose.connection.on("connected", () => {
  console.log("Database connected");
});
