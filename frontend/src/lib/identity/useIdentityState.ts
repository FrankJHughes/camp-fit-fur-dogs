import { useContext } from "react";
import { IdentityContext } from "./identityContext";

export function useIdentityState() {
  return useContext(IdentityContext);
}
