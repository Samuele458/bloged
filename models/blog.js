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

const CategorySchema = new mongoose.Schema({
  urlName: {
    type: String,
    required: true,
  },
  fullName: {
    type: String,
    required: true,
  },
  description: String,
  blog: { type: mongoose.Schema.Types.ObjectId, ref: "Blog" },
});

mongoose.model("Category", CategorySchema);

const TagSchema = new mongoose.Schema({
  urlName: {
    type: String,
    required: true,
  },
  fullName: {
    type: String,
    required: true,
  },
  description: String,
  blog: { type: mongoose.Schema.Types.ObjectId, ref: "Blog" },
});

mongoose.model("Tag", TagSchema);

const PostSchema = new mongoose.Schema({
  urlName: {
    type: String,
    required: true,
  },
  fullName: {
    type: String,
    required: true,
  },
  text: String,
  blog: { type: mongoose.Schema.Types.ObjectId, ref: "Blog" },
  tags: [{ type: mongoose.Schema.Types.ObjectId, ref: "Tag" }],
  //categories: [{ type: mongoose.Schema.Types.ObjectId, ref: "Category" }],
});

mongoose.model("Post", PostSchema);

const ImageSchema = new mongoose.Schema({
  description: String,
  title: String,
  urlName: { required: true, type: String },
  key: {
    type: String,
    required: true,
  },
});

mongoose.model("Image", ImageSchema);
