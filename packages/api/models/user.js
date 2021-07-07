const mongoose = require("mongoose");

const UserSchema = new mongoose.Schema({
  username: {
    type: String,
    required: true,
  },
  hash: {
    type: String,
    required: true,
  },
  salt: {
    type: String,
    required: true,
  },
  email: {
    type: String,
    required: true,
  },
  role: {
    type: String,
    default: "subscriber",
  },
  active: {
    type: Boolean,
    default: false,
  },
});

mongoose.model("User", UserSchema);

const VerificationSchema = new mongoose.Schema({
  verificationString: { type: String, required: true },
  user: { type: mongoose.Schema.Types.ObjectId, ref: "User" },
});

mongoose.model("Verification", VerificationSchema);
