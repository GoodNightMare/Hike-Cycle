import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.jsx";
import { CartProvider } from "./context/CartContext.jsx";
import { PromotionProvider } from "./context/PromotionContext.jsx";

createRoot(document.getElementById("root")).render(
  <StrictMode>
    <CartProvider>
      <PromotionProvider>
        <App />
      </PromotionProvider>
    </CartProvider>
  </StrictMode>
);
