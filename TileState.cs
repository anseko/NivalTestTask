using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileStates
{

    public abstract class TileState
    {
        public abstract void OnPointerClick(int button, Tile tile);
        public abstract void OnEnter(Tile tile);
        public abstract void Update(Tile tile);
        public abstract void OnExit(Tile tile);
    }

    public class IdleTileState : TileState
    {
        public override void Update(Tile tile)
        {
        }

        public override void OnEnter(Tile tile)
        {
        }

        public override void OnExit(Tile tile)
        {
            tile.prevState = tile.GetState();
        }

        public override void OnPointerClick(int button, Tile tile)
        {
            if(button == 0)
            {
                tile.SetState(Tile.States.BUSY);
            }

            if (button == 1)
            {
                tile.SetState(Tile.States.AVALIABLE);
            }

        }
    }

    public class AvailableTileState : TileState
    {
        
        public override void Update(Tile tile)
        {

        }

        public override void OnEnter(Tile tile)
        {
            //окрасить в нужный цвет; добавить в список доступных
            tile.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            if (!Tile.availableTiles.Contains(tile))
            {
                Tile.availableTiles.Add(tile);
            }
        }

        public override void OnExit(Tile tile)
        {
            //покрасить в обратный цвет; удалить из списка доступных
            tile.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            tile.prevState = tile.GetState();
            if (Tile.availableTiles.Contains(tile))
            {
                Tile.availableTiles.Remove(tile);
            }
        }

        public override void OnPointerClick(int button, Tile tile)
        {
            if (button == 0)
            {
                tile.SetState(Tile.States.BUSY);
            }

            if (button == 1)
            {
                tile.SetState(Tile.States.IDLE);
            }
        }
    }

    public class BusyTileState : TileState
    {

        public override void Update(Tile tile)
        {
        }

        public override void OnEnter(Tile tile)
        {
            //создать куб
            tile.StartCoroutine(Arise(tile));
        }

        public override void OnExit(Tile tile)
        {
            //удалить куб 
            tile.StartCoroutine(Dissolve(tile));
            tile.prevState = tile.GetState();
        }

        public override void OnPointerClick(int button, Tile tile)
        {
            if (button == 0)
            {
                if (tile.prevState == Tile.States.AVALIABLE)
                {
                    tile.SetState(Tile.States.AVALIABLE);
                }
                else { 
                    tile.SetState(Tile.States.IDLE);
                }
            }
        }

        private static IEnumerator Arise(Tile tile)
        {
            tile.cube.SetActive(true);
            Material cubeMaterial = tile.cube.GetComponent<Renderer>().material;
            cubeMaterial.SetFloat("_DissolveAmount", 1);


            float t = 1;
            while (t > 0)
            {
                cubeMaterial.SetFloat("_DissolveAmount", t);
                t -= 0.03f;
                yield return new WaitForSecondsRealtime(0.01f);
            }
            yield return null;
        }

        private static IEnumerator Dissolve(Tile tile)
        {
            Material cubeMaterial = tile.cube.GetComponent<Renderer>().material;
            float t = 0;
            while (t < 1)
            {
                cubeMaterial.SetFloat("_DissolveAmount", t);
                t += 0.03f;
                yield return new WaitForSecondsRealtime(0.01f);
            }

            tile.cube.SetActive(false);
            yield return null;
        }
    }

}