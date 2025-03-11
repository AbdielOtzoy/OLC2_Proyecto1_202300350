'use client'

import TextEditor from "@/components/TextEditor";
import { useState } from "react";
import { toast } from "sonner"


export default function Home() {
  const [code, setCode] = useState('');
  const [output, setOutput] = useState('');

  const handleExecute = async () => {
    try {
      console.log(code);
      const res = await fetch('http://localhost:5215/compile', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ code })
      });
      
      console.log(res);
      const data = await res.json();
      console.log(data);

      if(!res.ok) {
        toast.error(data.message || 'Something went wrong');
        throw new Error(data.message || 'Something went wrong');
      }

      setOutput(data.result);
      

    } catch (error) {
      console.log(error);
      setOutput('');
      
    }
  }

  return (
    <div className="flex h-screen bg-[#131313] align-center justify-center gap-2">

      <TextEditor 
        code={code}
        setCode={setCode}
        handleExecute={handleExecute}
      />

      <div className='bg-[#1e1e1e] text-white p-4 h-[80vh] flex flex-col gap-4 w-1/3 mt-[91px] mr-3 overflow-auto'>
        <h2>Output</h2>
        <pre>{output}</pre>
      </div >



    </div>
  );
}