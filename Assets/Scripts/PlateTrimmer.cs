using System;
using System.Collections.Generic;
using System.Linq;
using Parabox.CSG;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using Object = UnityEngine.Object;

namespace Scripts
{
    [Serializable]
    public class PlateTrimmer
    {
        private Floor FloorObj => Level.level.floor;
        
        [SerializeField] private GameObject plateModel;
        [SerializeField] private Vector3 plateSizes;

        public float overlapHeight;
        public LayerMask trimmersMask;
        public LayerMask platesMask;

        private Vector3 position;
        private Model model;
        
        public void Set(Model m, Vector3 p)
        {
            model = m;
            position = p;
        }
        
        public void PreparePlate()
        {
            List<RaycastHit> hits = RaycastUnderPlate();

            if (hits.Count != 0)
            {
                if (hits.Any(x => x.collider.CompareTag("Floor")) &&
                    hits.Any(x => x.collider.CompareTag("Ground")))
                {
                    CutoffOddFromFloor();
                    TrimByBorder();
                    TrimByPlates();
                    
                    CreatePlate();
                }
                else if (hits.Any(x => x.collider.CompareTag("Floor")) &&
                         !hits.Any(x => x.collider.CompareTag("Ground")))
                {
                    TrimByPlates();
                    
                    CreatePlate();
                } else 
                {
                    CreateOdd(model);
                }
            }
        }

        private void TrimByPlates()
        {
            List<Collider> plates = Physics.OverlapBox(
                    position - new Vector3(0f, overlapHeight / 2f, 0f), 
                    plateSizes / 2 + new Vector3(0f, overlapHeight - plateSizes.y / 2f, 0f), 
                    Quaternion.identity, 
                    platesMask)
                .ToList();


            try
            {
                foreach (Collider plate in plates)
                {
                    Model tempPlateModel = new Model(plate.gameObject);

                    CutoffOddFromPlate(tempPlateModel);
                    TrimByPlate(tempPlateModel);
                }
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        private void TrimByPlate(Model tempPlateModel)
        {
            model = CSG.Perform(CSG.BooleanOp.Subtraction, model, tempPlateModel);
        }

        private void CutoffOddFromPlate(Model objModel)
        {
            Model result = CSG.Perform(CSG.BooleanOp.Intersection, model, objModel);
            CreateOdd(result);
        }

        private void CutoffOddFromFloor()
        {
            Model floorModel = new Model(FloorObj.floorBase.gameObject);
            Model result = CSG.Perform(CSG.BooleanOp.Subtraction, model, floorModel);
            CreateOdd(result);
        }

        private void TrimByBorder()
        {
            Model floorModel = new Model(FloorObj.floorBase.gameObject);
            model = CSG.Perform(CSG.BooleanOp.Intersection, model, floorModel);
        }
        
        private List<RaycastHit> RaycastUnderPlate()
        {
            RaycastHit hit;
            List<RaycastHit> hits = new List<RaycastHit>();
        
            Vector3 _leftTop = position + new Vector3(plateSizes.x / 2f, 0f, plateSizes.z / 2f);
            Vector3 _leftBottom = position + new Vector3(-plateSizes.x / 2f, 0f, plateSizes.z / 2f);
            Vector3 _rightTop = position + new Vector3(plateSizes.x / 2f, 0f, -plateSizes.z / 2f);
            Vector3 _rightBottom = position + new Vector3(-plateSizes.x / 2f, 0f, -plateSizes.z / 2f);
        
            if(Physics.Raycast(_leftTop, Vector3.down, out hit, overlapHeight , trimmersMask)) hits.Add(hit);
            if(Physics.Raycast(_leftBottom, Vector3.down, out hit, overlapHeight , trimmersMask)) hits.Add(hit);
            if(Physics.Raycast(_rightTop, Vector3.down, out hit, overlapHeight , trimmersMask)) hits.Add(hit);
            if(Physics.Raycast(_rightBottom, Vector3.down, out hit, overlapHeight , trimmersMask)) hits.Add(hit);

            return hits;
        }

        private void CreatePlate()
        {
            model.CenterPivotByOffset(new Vector3(
                position.x,
                0f,
                position.z));
            
            GameObject obj = Object.Instantiate(plateModel, position, Quaternion.identity);

            obj.GetComponent<MeshFilter>().sharedMesh = (Mesh) model;
            obj.GetComponent<MeshRenderer>().materials = model.materials.ToArray();

            MeshImporter importer = new MeshImporter(obj);
            importer.Import(new MeshImportSettings() {quads = true, smoothing = true, smoothingAngle = 1f});
            
            obj.transform.position = position;
            obj.transform.parent = FloorObj.floorFill;
            obj.gameObject.AddComponent<MeshCollider>().convex = true;
            obj.gameObject.AddComponent<Rigidbody>().constraints = 
                (RigidbodyConstraints) 122;
            obj.gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
                obj.layer = LayerMask.NameToLayer("PlacedPlate");
        }
        
        
        private void CreateOdd(Model reference)
        {
            Model oddModel = reference;
            
            reference.CenterPivotByOffset(new Vector3(
                position.x,
                0f,
                position.z));
            
            ProBuilderMesh odd = ProBuilderMesh.Create();

            odd.GetComponent<MeshFilter>().sharedMesh = (Mesh) oddModel;
            odd.GetComponent<MeshRenderer>().materials = oddModel.materials.ToArray();

            MeshImporter importer = new MeshImporter(odd.gameObject);
            importer.Import(new MeshImportSettings() {quads = true, smoothing = true, smoothingAngle = 1f});

            odd.transform.position = position;
            odd.transform.parent = FloorObj.floorTrash;
            odd.gameObject.AddComponent<MeshCollider>().convex = true;
            odd.gameObject.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            //odd.gameObject.GetComponent<Rigidbody>().AddExplosionForce(20, Vector3.zero, 10);
            
            FloorObj.ClearTrash(odd.gameObject);
        }

    }
}