// Browser side — WebAuthn API. Uses base64url helpers; in real code use @simplewebauthn/browser.

export async function registerPasskey(email: string): Promise<void> {
  // 1) Ask server for options
  const begin = await fetch('/passkeys/register/begin', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email }),
  });
  const options = await begin.json();

  // Decode b64url → ArrayBuffer for the browser API
  options.challenge = b64urlToBuf(options.challenge);
  options.user.id = b64urlToBuf(options.user.id);
  options.excludeCredentials?.forEach((c: any) => (c.id = b64urlToBuf(c.id)));

  const credential = (await navigator.credentials.create({ publicKey: options })) as PublicKeyCredential;
  const r = credential.response as AuthenticatorAttestationResponse;

  await fetch('/passkeys/register/finish', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      id: credential.id,
      rawId: bufToB64url(credential.rawId),
      type: credential.type,
      response: {
        clientDataJSON: bufToB64url(r.clientDataJSON),
        attestationObject: bufToB64url(r.attestationObject),
      },
    }),
  });
}

export async function authenticateWithPasskey(email: string): Promise<void> {
  const begin = await fetch('/passkeys/login/begin', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email }),
  });
  const options = await begin.json();
  options.challenge = b64urlToBuf(options.challenge);
  options.allowCredentials?.forEach((c: any) => (c.id = b64urlToBuf(c.id)));

  const assertion = (await navigator.credentials.get({ publicKey: options })) as PublicKeyCredential;
  const r = assertion.response as AuthenticatorAssertionResponse;

  await fetch('/passkeys/login/finish', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      id: assertion.id,
      rawId: bufToB64url(assertion.rawId),
      type: assertion.type,
      response: {
        clientDataJSON: bufToB64url(r.clientDataJSON),
        authenticatorData: bufToB64url(r.authenticatorData),
        signature: bufToB64url(r.signature),
        userHandle: r.userHandle ? bufToB64url(r.userHandle) : null,
      },
    }),
  });
}

function b64urlToBuf(b64url: string): ArrayBuffer {
  const b64 = b64url.replace(/-/g, '+').replace(/_/g, '/').padEnd(b64url.length + ((4 - (b64url.length % 4)) % 4), '=');
  const bin = atob(b64);
  const buf = new Uint8Array(bin.length);
  for (let i = 0; i < bin.length; i++) buf[i] = bin.charCodeAt(i);
  return buf.buffer;
}

function bufToB64url(buf: ArrayBuffer): string {
  const bytes = new Uint8Array(buf);
  let bin = '';
  for (let i = 0; i < bytes.byteLength; i++) bin += String.fromCharCode(bytes[i]);
  return btoa(bin).replace(/\+/g, '-').replace(/\//g, '_').replace(/=/g, '');
}
