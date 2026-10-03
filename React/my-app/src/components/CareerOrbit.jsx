function CareerOrbit({ skillCount, jobCount }) {
  return (
    <div className="career-orbit-wrap">
      <p className="sr-only">Interactive visual representing {skillCount} detected skills and {jobCount} saved jobs.</p>
      <div className="career-orbit" aria-hidden="true">
        <div className="orbit-glow" />
        <div className="orbit-grid" />
        <div className="orbit-ring ring-one"><span className="orbit-node node-react">React</span></div>
        <div className="orbit-ring ring-two"><span className="orbit-node node-dotnet">.NET</span></div>
        <div className="orbit-ring ring-three"><span className="orbit-node node-data">SQL</span></div>
        <div className="orbit-core">
          <span className="core-mark">S</span>
          <div><strong>{skillCount}</strong><small>skills mapped</small></div>
        </div>
        <span className="orbit-float float-cloud">Cloud</span>
        <span className="orbit-float float-devops">DevOps</span>
        <span className="orbit-float float-api">API</span>
      </div>
    </div>
  )
}

export default CareerOrbit
