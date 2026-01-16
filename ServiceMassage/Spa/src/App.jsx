import Router from "./Route/index";
import { CartProvider } from "./Store/CartContext";

export default function App() {
  return (
    <CartProvider>
      <Router />
    </CartProvider>
  );
}
