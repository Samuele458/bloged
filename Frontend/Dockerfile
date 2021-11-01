FROM node:17

WORKDIR /app

COPY . .

RUN npm update -g yarn
RUN npm install -g lerna
RUN yarn
RUN lerna bootstrap
RUN yarn build --cwd /app/packages/dashboard
RUN yarn build --cwd /app/packages/blog

RUN chmod +x ./deploy.sh
