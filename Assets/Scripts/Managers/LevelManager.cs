using Commands.Level;
using Data.UnityObjects;
using Data.ValueObjects;
using UnityEngine;
using Signals;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        #region Self Variables
        
        #region Serialized Variables
        
        [SerializeField] private Transform levelHolder;
        [SerializeField] private byte totalLevelCount;
        
        #endregion
        
        #region Private Variables
        
        private OnLevelLoaderCommand _levelLoaderCommand;
        private OnLevelDestroyerCommand _levelDestroyerCommand;
        
        private LevelData _levelData;
        private ushort _currentLevel;
        
        #endregion
        
        #endregion

        #region Unity Callbacks
        
        private void Awake()
        {
            _levelData = GetLevelData();
            _currentLevel = GetActiveLevel();

            Init();
        }
        
        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void Start()
        {
            CoreGameSignals.Instance.onLevelInitialize?.Invoke((byte)(_currentLevel % totalLevelCount));
            //UISignals
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }
        
        #endregion
        
        #region Private Methods

        private LevelData GetLevelData()
        {
            return Resources.Load<CD_Level>("Data/CD_Level").Levels[_currentLevel];
        }
        
        private byte GetActiveLevel()
        {
            return (byte)_currentLevel;
        }
        
        private void Init()
        {
            _levelLoaderCommand = new OnLevelLoaderCommand(levelHolder);
            _levelDestroyerCommand = new OnLevelDestroyerCommand(levelHolder);
        }
        
        private void OnNextLevel()
        {
            _currentLevel++;
            OnRestartLevel();
        }

        private void OnRestartLevel()
        {
            CoreGameSignals.Instance.onClearActiveLevel?.Invoke();
            CoreGameSignals.Instance.onResetLevel?.Invoke();
            CoreGameSignals.Instance.onLevelInitialize?.Invoke((byte)(_currentLevel % totalLevelCount));
        }

        private byte OnGetLevelValue()
        {
            return (byte)_currentLevel;
        }
        
        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.onLevelInitialize += _levelLoaderCommand.Execute;
            CoreGameSignals.Instance.onClearActiveLevel += _levelDestroyerCommand.Execute;
            CoreGameSignals.Instance.onGetLevelValue += OnGetLevelValue;
            CoreGameSignals.Instance.onNextLevel += OnNextLevel;
            CoreGameSignals.Instance.onRestartLevel += OnRestartLevel;
        }

        private void UnSubscribeEvents()
        {
            CoreGameSignals.Instance.onLevelInitialize -= _levelLoaderCommand.Execute;
            CoreGameSignals.Instance.onClearActiveLevel -= _levelDestroyerCommand.Execute;
            CoreGameSignals.Instance.onGetLevelValue -= OnGetLevelValue;
            CoreGameSignals.Instance.onNextLevel -= OnNextLevel;
            CoreGameSignals.Instance.onRestartLevel -= OnRestartLevel;
        }
        
        #endregion
    }
}