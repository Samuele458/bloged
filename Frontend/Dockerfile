FROM node:17-alpine

WORKDIR /app

COPY . .

RUN npm install -g yarn
RUN yarn install
RUN yarn run build

RUN chmod +x ./deploy.sh
