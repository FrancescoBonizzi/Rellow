import { defineConfig } from "vite";

export default defineConfig({
  build: {
    rollupOptions: {
      input: "index.html",
    },
  },
  base: "/rellow-play", // Per nextJs
});
