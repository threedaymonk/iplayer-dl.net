TESTS   = Dir["test/**/*.cs"]
SOURCES = Dir["src/**/*.cs"]
ASSEMBLIES = %w[System.Xml.Linq System.Core]

def gmcs(*items)
  system *(
    ["gmcs"] +
    ASSEMBLIES.map{ |a| "-r:#{a}.dll" } +
    items
  )
  raise "Compilation failed" unless $? == 0
end

file "test.dll" => TESTS + SOURCES do |t|
  gmcs "-t:library", "-pkg:nunit", "-out:test.dll", *(SOURCES + TESTS)
end

task :test => "test.dll" do
  system "nunit-console", "test.dll"
  rm_rf '%temp%'
end
