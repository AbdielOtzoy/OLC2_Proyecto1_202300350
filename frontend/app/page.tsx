'use client'

import TextEditor from "@/components/TextEditor";
import api from "@/lib/api";
import { useState } from "react";

export default function Home() {
  const [code, setCode] = useState('');
  const [output, setOutput] = useState('');

  const handleExecute = async () => {
    try {
      console.log(code);
      const res = await api.post('/compile', { code });
      console.log(res);
      setOutput(res.data);

    } catch (error) {
      console.log(error);
      
    }
  }

  return (
    <div className="flex h-screen bg-[#131313] align-center justify-center gap-2">

      <TextEditor 
        code={code}
        setCode={setCode}
        handleExecute={handleExecute}
      />

      {/* terminal to show the output */}
      <div className='bg-[#1e1e1e] text-white p-4 h-[80vh] flex flex-col gap-4 w-1/3 mt-[91px]'>
        <h1>Output</h1>
        <pre>{output}</pre>
      </div >


    </div>
  );
}
