import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

export default function middleware(req: NextRequest) {
  const session = req.cookies.get('cffd.session');
  const isAuthenticated = !!session;

  const { pathname } = req.nextUrl;

  // Public routes that never require authentication
  const publicRoutes = new Set([
    '/',
    '/logged-out'
  ]);

  const isPublicRoute = publicRoutes.has(pathname);

  // If unauthenticated and not a public route → redirect to login
  if (!isAuthenticated && !isPublicRoute) {
    const loginUrl = new URL('/', req.url);
    loginUrl.searchParams.set('returnUrl', pathname);
    return NextResponse.redirect(loginUrl);
  }

  return NextResponse.next();
}
