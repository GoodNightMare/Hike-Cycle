// src/layouts/AdminLayout.jsx
import Sidebar from "../components/Sidebar";
import Topbar from "../components/Topbar";

export default function AdminLayout({ children }) {
  return (
    <div className="flex min-h-screen w-full bg-gray-100">
      <Sidebar />

      <div className="flex flex-col flex-1 min-w-0">
        <Topbar />

        <main className="flex-1 overflow-y-auto ">
          {children}
        </main>
      </div>
    </div>
  );
}
