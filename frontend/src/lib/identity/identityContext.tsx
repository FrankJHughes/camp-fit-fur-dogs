'use client';

import { createContext } from 'react';
import { IdentityState } from './identityState';

export const IdentityContext = createContext<IdentityState>({
  isAuthenticated: false,
  name: 'Anonymous',
});
