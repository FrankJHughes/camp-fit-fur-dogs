import { describe, it, expect } from 'vitest';
import { NextRequest } from 'next/server';
import middleware from '@/middleware';

function exec(url: string, cookie?: string) {
  const req = new NextRequest(new URL(url));
  if (cookie) req.cookies.set('cffd.session', cookie);
  return middleware(req);
}

describe('middleware', () => {
  it('allows public routes', () => {
    const res = exec('http://localhost/login');

    expect(res?.status).toBe(200);
    expect(res?.headers.get('x-middleware-next')).toBe('1');
  });

  it('allows logged-out page', () => {
    const res = exec('http://localhost/logged-out');

    expect(res?.status).toBe(200);
    expect(res?.headers.get('x-middleware-next')).toBe('1');
  });

  it('blocks authenticated routes without cookie', () => {
    const res = exec('http://localhost/dogs');

    expect(res?.status).toBe(307);
    expect(res?.headers.get('Location')).toMatch(/^http:\/\/localhost\/login/);
  });

  it('allows authenticated routes with cookie', () => {
    const res = exec('http://localhost/dogs', 'abc123');

    expect(res?.status).toBe(200);
    expect(res?.headers.get('x-middleware-next')).toBe('1');
  });
});
