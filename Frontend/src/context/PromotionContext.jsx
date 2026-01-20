import { createContext, useContext, useState } from "react";

const PromotionContext = createContext();

export function PromotionProvider({ children }) {
  const [promotion, setPromotion] = useState(null);

  const applyPromotion = (promo) => {
    setPromotion(promo);
  };

  const clearPromotion = () => {
    setPromotion(null);
  };

  return (
    <PromotionContext.Provider
      value={{
        promotion,
        applyPromotion,
        clearPromotion,
      }}
    >
      {children}
    </PromotionContext.Provider>
  );
}

export function usePromotion() {
  return useContext(PromotionContext);
}
