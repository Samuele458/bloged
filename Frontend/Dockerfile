FROM node:17

WORKDIR /app

COPY . .

RUN npm update -g yarn
RUN npm install -g lerna
RUN yarn
RUN lerna bootstrap
RUN yarn build

RUN chmod +x ./deploy.sh
