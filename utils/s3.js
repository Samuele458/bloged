require("dotenv").config();
const S3 = require("aws-sdk/clients/s3");
const fs = require("fs");

//reading S3 info from env file
const bucketName = process.env.AWS_BUCKET_NAME;
const region = process.env.AWS_BUCKET_REGION;
const accessKeyId = process.env.AWS_ACCESS_KEY;
const secretAccessKey = process.env.AWS_SECRET_KEY;

//connecting to S3
const s3 = new S3({
  region,
  accessKeyId,
  secretAccessKey,
});

/**
 * Upload a file to S3
 *
 * @param {object} file - It is the object file created by multer (it needs the buffer field), and also the filename field is required, so you need to add it before sending file to this function
 * @returns promise
 */
module.exports.uploadToS3 = (file) => {
  const uploadParams = {
    Bucket: bucketName,
    Body: file.buffer,
    Key: file.filename,
  };

  return s3.upload(uploadParams).promise();
};

/**
 *
 * @param {string} fileKey - File identifier, in order to download the file
 * @returns file stream
 */
module.exports.downloadFromS3 = (fileKey) => {
  const downloadParams = {
    Key: fileKey,
    Bucket: bucketName,
  };

  return s3.getObject(downloadParams).createReadStream();
};
