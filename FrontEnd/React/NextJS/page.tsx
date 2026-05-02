// app/posts/page.tsx — Server Component (default in App Router)
import { Suspense } from 'react';
import { createPost } from './actions';

export const revalidate = 60; // ISR: revalidate every 60s

async function getPosts() {
  const res = await fetch('https://api.example.com/posts', {
    next: { tags: ['posts'] }, // tag-based revalidation
  });
  if (!res.ok) throw new Error('Failed to load posts');
  return res.json() as Promise<{ id: string; title: string }[]>;
}

export default async function PostsPage() {
  const posts = await getPosts();
  return (
    <main>
      <h1>Posts</h1>
      <Suspense fallback={<p>Loading...</p>}>
        <ul>
          {posts.map((p) => (
            <li key={p.id}>{p.title}</li>
          ))}
        </ul>
      </Suspense>

      <form action={createPost}>
        <input name="title" required />
        <button type="submit">Add</button>
      </form>
    </main>
  );
}
