const mongoose = require("mongoose");

const BlogSchema = new mongoose.Schema({
  urlName: {
    type: String,
    required: true,
  },
  fullName: {
    type: String,
    required: true,
  },
  admins: [{ type: mongoose.Schema.Types.ObjectId, ref: "User" }],
});

mongoose.model("Blog", BlogSchema);
